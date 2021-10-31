using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyPet.Api.Models;
using MyPet.BLL.DTO;
using MyPet.BLL.Interfaces;

using System;
using System.Collections.Generic;
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

        public AdvertisementController(ILogger<AdvertisementController> logger, IAdvertisementService adservice)
        {
            _logger = logger;
            this.adService = adservice;
        }

        [HttpPut]
        public async Task<IActionResult> AddAdvertisement([FromForm]AdvertisementModel model)
        {
            string folderToSave = Path.Combine(Directory.GetCurrentDirectory(), "Resourses", "Images");
            long size = model.Images.Sum(f => f.Length);

            AdvertisementDTO admodel = new AdvertisementDTO
            {
                UserId = model.UserId,
                UserName = model.UserName,
                Description = model.Description,
                PetName = model.PetName,
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

            adService.AddAdvertisement(admodel);

            return new ObjectResult(new { imagesCount = model.Images.Count, size }) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet]
        public IActionResult GetAdvertisementById(int id)
        {
            var ad =  adService.GetAdvertisementById(id);

            if (ad != null)
                return new ObjectResult(ad);
            else
                ModelState.AddModelError("id", "Ad not found");
                return ValidationProblem(ModelState);
        }

        [HttpGet]
        public IActionResult GetAllAdvertisement()
        {
            var ads = adService.GetAllAdvertisements();

            if (ads.Count() > 0)
                return Ok(ads.ToArray());
            else
                return BadRequest();
        }
    }

}
