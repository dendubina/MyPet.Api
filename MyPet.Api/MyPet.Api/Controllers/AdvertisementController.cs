using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public AdvertisementController(ILogger<AdvertisementController> logger, IAdvertisementService adservice, IMapper mapper)
        {
            _logger = logger;
            this.adService = adservice;
            this.mapper = mapper;
        }

        [HttpPut]
        public async Task<IActionResult> AddAdvertisementAsync([FromForm]AdvertisementModel model)
        {
            string folderToSave = Path.Combine(Directory.GetCurrentDirectory(), "Resourses", "Images");
            long size = model.Images.Sum(f => f.Length);

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

            foreach (var formFile in model.Images)
            {
                if (formFile.Length > 0)
                {
                    string filename = Path.GetRandomFileName();
                    filename = Path.GetFileNameWithoutExtension(filename);
                    filename = filename + Path.GetExtension(formFile.FileName);


                    string fullpath = Path.Combine(folderToSave, filename);

                    admodel.Images.Add(new ImageDTO {
                        Size = formFile.Length,
                        Path = fullpath,
                    });

                    using (var stream = System.IO.File.Create(fullpath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

           await adService.AddAdvertisementAsync(admodel);

            return new ObjectResult(new { imagesCount = model.Images.Count, size }) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet]
        public async Task<IActionResult> GetAdvertisementByIdAsync([Required]int id)
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
        public async Task<IActionResult> GetAllAdvertisementsAsync()
        {
            var ads = await adService.GetAllAdvertisementsAsync();

            var result = mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);
                       
            if (result.Count() > 0)
                return Ok(result);
            else
                return BadRequest();
        }
    }

}
