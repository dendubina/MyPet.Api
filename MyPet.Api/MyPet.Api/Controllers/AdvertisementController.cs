using AutoMapper;
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
        private readonly UserManager<IdentityUser> userManager;

        public AdvertisementController(ILogger<AdvertisementController> logger, IAdvertisementService adService, IMapper mapper, IWebHostEnvironment env, IConfiguration config, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            this.adService = adService;
            this.mapper = mapper;
            webHostEnvironment = env;
            this.config = config;
            this.userManager = userManager;
        }

        [Authorize]
        [HttpPut]        
        public async Task<IActionResult> AddAdvertisement([FromForm] AdvertisementModel model)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var user = await userManager.FindByIdAsync(userId);

            string fullpath = await AddImageGetPath(model.Image);
           
            PetDTO petDto = new PetDTO
            {
                Name = model.PetName,
                Location = mapper.Map<LocationDTO>(model)
            };
            AdvertisementDTO admodel = new AdvertisementDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
                Description = model.Description,
                Category = model.Category,
                Pet = petDto,
                Images = new List<ImageDTO>(),
            };
            admodel.Images.Add(new ImageDTO
            {
                Size = model.Image.Length,
                Path = fullpath,
            });            


            await adService.AddAdvertisementAsync(admodel);

            var responseModel = new
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
                PublicationDate = DateTime.Now,
                Description = model.Description,
                Category = model.Category,
                LocationRegion = model.LocationRegion,
                LocationStreet = model.LocationStreet,
                LocationTown = model.LocationTown,
                LocationHouse = model.LocationHouse,
                PetName = model.PetName,
                ImagePath = fullpath,
                ImageSize = model.Image.Length,
                status = 201,
            };

            return new ObjectResult(responseModel) { StatusCode = StatusCodes.Status201Created };

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAdvertisements()
        {
            var ads = await adService.GetAllAdvertisementsAsync();

            var result = mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);
            
            return Ok(result);            
        }

        [HttpGet]
        [AllowAnonymous]       
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
        [AllowAnonymous]
        public async Task<IActionResult> GetAdsPagedList([FromQuery] AdPagedRequestParameters parameters)
        {
            var ads = await adService.GetFilteredPagedAdvertisementsAsync(parameters.PageNumber, parameters.PageSize, parameters.LocationRegion, parameters.Category, parameters.LocationTown);
            var result = mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);
            
            return Ok(result);            
        }

        [Authorize]
        [HttpGet]        
        public async Task<IActionResult> GetAdsByUser()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            
            var ads = await adService.GetAdsByUserAsync(userId);
            var result = mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);
            
            return Ok(result);           
        }

        [Authorize]
        [HttpGet]        
        public async Task<IActionResult> GetUsersAdsPagedList([FromQuery] AdPagedRequestParameters parameters)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;            

            var ads = await adService.GetPagedAdsByUserAsync(userId, parameters.PageNumber, parameters.PageSize);
            var result = mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);
            
            return Ok(result);            
        }

        [Authorize]
        [HttpDelete]        
        public async Task<IActionResult> DeleteAdvertisement([Required] int id)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var user = await userManager.FindByIdAsync(userId);
            var ad = await adService.GetAdvertisementByIdAsync(id);

            if(ad == null || userId != ad.UserId)
            {
                ModelState.AddModelError("error", "something went wrong");
                return ValidationProblem(ModelState);
            }

            var deletedAd = await adService.DeleteAdvertisementAsync(id);

            var responseModel = mapper.Map<AdvertisementResponseModel>(deletedAd);

            return Ok(responseModel);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ApproveAdStatus ([Required] int id)
        {
            var ad = await adService.ApproveAdStatus(id);
            var result = mapper.Map<AdvertisementResponseModel>(ad);

            return Ok(result);
        }

        [Authorize]
        [HttpPost]        
        public async Task<IActionResult> UpdateAdvertisement([FromForm] UpdatedAdvertisementModel model)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;           

            LocationDTO locDto = new LocationDTO
            {
                Region = model.LocationRegion,
                Town = model.LocationTown,
                Street = model.LocationStreet,
                House = model.LocationHouse,
            };
            PetDTO petDto = new PetDTO
            {
                Name = model.PetName,
                Location = locDto,
            };
            AdvertisementDTO newAd = new AdvertisementDTO
            {   
                Id = model.AdId,
                Description = model.Description,
                Category = model.Category,
                Pet = petDto,               
            };

            if (model.Image != null)
            {
                newAd.Images = new List<ImageDTO>();
                string pathToImg = await AddImageGetPath(model.Image);
                newAd.Images.Add(new ImageDTO
                {
                    Path = pathToImg,
                    Size = model.Image.Length,
                });
            }          

            var updatedAd = await adService.UpdateAdvetrtisementAsync(newAd, userId);

            var responseModel = mapper.Map<AdvertisementResponseModel>(updatedAd);            

            return new ObjectResult(responseModel) { StatusCode = StatusCodes.Status201Created };
        }
        

        private async Task<string> AddImageGetPath(IFormFile image)
        {
            string ImagesFolder = config["ImagesFolder"];
            string folderToSave = webHostEnvironment.WebRootPath + ImagesFolder; 

            string filename = Path.GetRandomFileName();

            filename = Path.GetFileNameWithoutExtension(filename); 
            filename = filename + Path.GetExtension(image.FileName);

            string fullpath = Path.Combine(folderToSave, filename);

            using (var stream = System.IO.File.Create(fullpath)) 
            {                
                await image.CopyToAsync(stream);
            }

            return ImagesFolder + filename; 
        }
    }
}
