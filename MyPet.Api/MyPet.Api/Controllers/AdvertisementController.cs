﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyPet.Api.Models;
using MyPet.BLL.DTO;
using MyPet.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyPet.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AdvertisementController : ControllerBase
    {
        private readonly ILogger<AdvertisementController> _logger;
        private readonly IAdvertisementService adService;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IConfiguration config;
        private readonly UserManager<IdentityUser> UserManager;

        public AdvertisementController(ILogger<AdvertisementController> logger, IAdvertisementService adservice, IMapper mapper, IWebHostEnvironment env, IConfiguration config, UserManager<IdentityUser> UserManager)
        {
            _logger = logger;
            this.adService = adservice;
            this.mapper = mapper;
            webHostEnvironment = env;
            this.config = config;
            this.UserManager = UserManager;
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> AddAdvertisement([FromForm] AdvertisementModel model)
        {
            string[] supportedTypes = new[] { "bmp", "png", "jpg", "jpeg", };
            string imgext = Path.GetExtension(model.Image.FileName).Substring(1);
            string ImagesFolder = config["ImagesFolder"];
            string folderToSave = webHostEnvironment.WebRootPath + ImagesFolder;

            if (!supportedTypes.Contains(imgext))
            {
                ModelState.AddModelError("Image", "Wrong file extension");
                return ValidationProblem(ModelState);
            }
            else if (model.Image.Length > (1048576 * Convert.ToInt32(config["MaxImageSizeMB"]))) //1MB * 5
            {
                ModelState.AddModelError("Image", $"Image size should be less than {config["MaxImageSizeMB"]}MB");
                return ValidationProblem(ModelState);
            }




            LocationDTO locDto = new LocationDTO
            {
                Town = model.LocationTown,
                Street = model.LocationStreet,
            };
            PetDTO petDto = new PetDTO
            {
                Name = model.PetName,
                Location = locDto,
            };
            AdvertisementDTO admodel = new AdvertisementDTO
            {
                UserId = model.UserId,
                UserName = model.UserName,
                Description = model.Description,
                Pet = petDto,
                Images = new List<ImageDTO>(),
            };


            string filename = Path.GetRandomFileName();
            filename = Path.GetFileNameWithoutExtension(filename);
            filename = filename + Path.GetExtension(model.Image.FileName);
            string fullpath = Path.Combine(folderToSave, filename);
            admodel.Images.Add(new ImageDTO
            {
                Size = model.Image.Length,
                Path = ImagesFolder + filename,
            });

            using (var stream = System.IO.File.Create(fullpath))
            {
                await model.Image.CopyToAsync(stream);
            }


              await adService.AddAdvertisementAsync(admodel);

            var responseModel = new
            {
                UserId = model.UserId,
                UserName = model.UserName,
                PublicationDate = DateTime.Now,
                Description = model.Description,
                LocationStreet = model.LocationStreet,
                LocationTown = model.LocationTown,
                PetName = model.PetName,
                ImagePath = ImagesFolder + filename,
                ImageSize = model.Image.Length,
                status = 201,
            };

            return new ObjectResult(responseModel) { StatusCode = StatusCodes.Status201Created };

        }

        [HttpGet]
        public async Task<IActionResult> GetAdvertisementById([Required] int id)
        {
            var ad = await adService.GetAdvertisementByIdAsync(id);
            var result = mapper.Map<AdvertisementResponseModel>(ad);

            if (result != null)
                return new ObjectResult(result);
            else
                ModelState.AddModelError("id", "Ad not found");
            return ValidationProblem(ModelState);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAdvertisements()
        {
            var ads = await adService.GetAllAdvertisementsAsync();

            var result = mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);

            if (result.Count() > 0)
                return Ok(result);
            else
                return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> GetAdsPagedList([FromQuery] AdPagedRequestParameters parameters)
        {
            var ads = await adService.GetPagedAdvertisementsAsync(parameters.PageNumber, parameters.PageSize);
            var result = mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);

            if (result.Count() > 0)
                return Ok(result);
            else
                return BadRequest();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAdsByUser()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var user = await UserManager.FindByIdAsync(userId);            

            if (user == null) {
                return BadRequest();
            }
            
             var ads = await adService.GetAdsByUserAsync(userId);
             var result = mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);


            if (result.Count() > 0)
                return Ok(result);
            else
                return BadRequest();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsersAdsPagedList([FromQuery] AdPagedRequestParameters parameters)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var user = await UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest();
            }

            var ads = await adService.GetPagedAdsByUserAsync(userId, parameters.PageNumber, parameters.PageSize);
            var result = mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);

            if (result.Count() > 0)
                return Ok(result);
            else
                return BadRequest();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteAdvertisement([Required] int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var user = await UserManager.FindByIdAsync(userId);
            var ad = await adService.GetAdvertisementByIdAsync(id);

            if(user == null || ad == null || userId != ad.UserId)
            {
                ModelState.AddModelError("error", "something went wrong");
                return ValidationProblem(ModelState);
            }

            var deletedAd = await adService.DeleteAdvertisementAsync(id);

            /*using (var stream = System.IO.File.Delete(fullpath))
            {
                await(stream);
            }*/

            return Ok(deletedAd);
        }
    }
}
