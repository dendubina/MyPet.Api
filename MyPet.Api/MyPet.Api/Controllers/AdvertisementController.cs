using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyPet.Api.Models;
using MyPet.BLL.DTO;
using MyPet.BLL.Interfaces;
using MyPet.BLL.Models.Ads;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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

        public AdvertisementController(ILogger<AdvertisementController> logger, IAdvertisementService adService, IMapper mapper, IWebHostEnvironment env, IConfiguration config)
        {
            _logger = logger;
            this.adService = adService;
            this.mapper = mapper;            
        }

        [Authorize]
        [HttpPost]        
        public async Task<IActionResult> AddAdvertisement([FromForm] AdvertisementModel model)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value;

            var result = await adService.AddAdvertisementAsync(model, userId);
            var responseModel = mapper.Map<AdvertisementResponseModel>(result);

            return Ok(responseModel);
        }

        /*[HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAdvertisements()
        {
            var ads = await adService.GetAllAdvertisementsAsync();

            var result = mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);
            
            return Ok(result);            
        }*/

        [HttpGet]
        [AllowAnonymous]       
        public async Task<IActionResult> GetAdvertisementById([Required] int id)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;

            var ad = await adService.GetAdvertisementByIdAsync(id, userId);

            var result = mapper.Map<AdvertisementResponseModel>(ad);
           
            return Ok(result);                
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
        public async Task<IActionResult> GetUsersAdsPagedList([FromQuery] AdPagedRequestParameters parameters)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value;

            var ads = await adService.GetPagedAdsByUserAsync(userId, parameters.PageNumber, parameters.PageSize);
            var result = mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);
            
            return Ok(result);            
        }

        [Authorize]
        [HttpDelete]        
        public async Task<IActionResult> DeleteAdvertisement([Required] int id)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value;
            var deletedAd = await adService.DeleteAdvertisementAsync(id, userId);

            var responseModel = mapper.Map<AdvertisementResponseModel>(deletedAd);

            return Ok(responseModel);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangeAdStatus (ChangeAdStatusModel model)
        {
            var ad = await adService.ChangeAdStatusAsync(model.AdId, model.Status);
            var result = mapper.Map<AdvertisementResponseModel>(ad);

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAdsOnModeration()
        {
            var ads = await adService.GetAdsOnModerationAsync();
            var result = mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateAdvertisement([FromForm] AdvertisementModel model)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value;          

            var updatedAd = await adService.UpdateAdvetrtisementAsync(model, userId);

            var responseModel = mapper.Map<AdvertisementResponseModel>(updatedAd);

            return Ok(responseModel);
        }

    }
}
