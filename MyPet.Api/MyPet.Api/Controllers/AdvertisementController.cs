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
using MyPet.Api.Extensions;

namespace MyPet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]    
    public class AdvertisementController : ControllerBase
    {
        private readonly IAdvertisementService _adService;
        private readonly IMapper _mapper;        

        public AdvertisementController(IAdvertisementService adService, IMapper mapper)
        {
            _adService = adService;
            _mapper = mapper;            
        }

        [Authorize]
        [HttpPost]        
        public async Task<IActionResult> AddAdvertisement([FromForm] AdvertisementModel model)
        {
            string userId = Request.GetUserId();
            
            var result = await _adService.AddAdvertisementAsync(model, userId);
            var responseModel = _mapper.Map<AdvertisementResponseModel>(result);

            return Ok(responseModel);
        }
       

        [HttpGet]
        [AllowAnonymous]       
        public async Task<IActionResult> GetAdvertisementById([Required] int id)
        {
            string userId = Request.GetUserId();

            var ad = await _adService.GetAdvertisementByIdAsync(id, userId);

            var result = _mapper.Map<AdvertisementResponseModel>(ad);
           
            return Ok(result);                
        }             

        [HttpGet]
        [AllowAnonymous]       
        public async Task<IActionResult> GetAdsPagedList([FromQuery] AdPagedRequestParameters parameters)
        {
            var ads = await _adService.GetFilteredPagedAdvertisementsAsync(parameters.PageNumber, parameters.PageSize, parameters.LocationRegion, parameters.Category, parameters.LocationTown);
            var result = _mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);
            
            return Ok(result);            
        }
        

        [Authorize]
        [HttpGet]        
        public async Task<IActionResult> GetUsersAdsPagedList([FromQuery] AdPagedRequestParameters parameters)
        {
            string userId = Request.GetUserId();

            var ads = await _adService.GetPagedAdsByUserAsync(userId, parameters.PageNumber, parameters.PageSize);
            var result = _mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);
            
            return Ok(result);            
        }

        [Authorize]
        [HttpDelete]        
        public async Task<IActionResult> DeleteAdvertisement([Required] int id)
        {
            string userId = Request.GetUserId();
            var deletedAd = await _adService.DeleteAdvertisementAsync(id, userId);

            var responseModel = _mapper.Map<AdvertisementResponseModel>(deletedAd);

            return Ok(responseModel);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ChangeAdStatus (ChangeAdStatusModel model)
        {
            var ad = await _adService.ChangeAdStatusAsync(model.AdId, model.Status);
            var result = _mapper.Map<AdvertisementResponseModel>(ad);

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAdsOnModeration()
        {
            var ads = await _adService.GetAdsOnModerationAsync();
            var result = _mapper.Map<IEnumerable<AdvertisementDTO>, IEnumerable<AdvertisementResponseModel>>(ads);

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateAdvertisement([FromForm] AdvertisementModel model)
        {
            string userId = Request.GetUserId();          

            var updatedAd = await _adService.UpdateAdvetrtisementAsync(model, userId);

            var responseModel = _mapper.Map<AdvertisementResponseModel>(updatedAd);

            return Ok(responseModel);
        }

    }
}
