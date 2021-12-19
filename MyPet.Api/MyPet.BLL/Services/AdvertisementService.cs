using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyPet.BLL.DTO;
using MyPet.BLL.Exceptions;
using MyPet.BLL.Interfaces;
using MyPet.DAL.Entities;
using MyPet.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyPet.BLL.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IAdvertisementRepository adRepo;
        private readonly IMapper mapper;
        private readonly ILogger<AdvertisementService> logger;
        private readonly UserManager<IdentityUser> userManager;
        
        private readonly bool isModerationEnabled;

       /* private const string OnModerationStatus = "OnModeration";
        private const string RejectedStatus = "Rejected";
        private const string ApprovedStatus = "Approved";*/

        private enum AdStatus
        {
            OnModeration,
            Rejected,
            Approved,
        };


        public AdvertisementService(IAdvertisementRepository adRepo, IMapper mapper, IConfiguration config, ILogger<AdvertisementService> logger, UserManager<IdentityUser> userManager)
        {
            this.adRepo = adRepo;
            this.mapper = mapper;
            this.logger = logger;
            this.userManager = userManager;
            isModerationEnabled = bool.Parse(config["EnableAdsPreModeration"]);
        }

        public async Task AddAdvertisementAsync(AdvertisementDTO model)
        {
            string status;

            if (isModerationEnabled)
                status = AdStatus.OnModeration.ToString();
            else
                status = AdStatus.Approved.ToString();


            Pet pet = mapper.Map<Pet>(model.Pet);           
            Advertisement ad = new Advertisement
            {
                UserId = model.UserId,
                UserName = model.UserName,
                UserEmail = model.UserEmail,
                Description = model.Description,
                Category = model.Category,
                Status = status,
                PublicationDate = DateTime.Now,
                Pet = pet,
                Images = mapper.Map<List<ImageDTO>,List<Image>>(model.Images),
            };            

           await adRepo.AddAsync(ad);
        }        

        public async Task<AdvertisementDTO> GetAdvertisementByIdAsync(int id)
        {
            var ad = await adRepo.GetByIdAsync(id);           

            return mapper.Map<AdvertisementDTO>(ad);
        }        

        public async Task<AdvertisementDTO> DeleteAdvertisementAsync(int id)
        {
            var ad = await adRepo.DeleteAsync(id);

            return mapper.Map<AdvertisementDTO>(ad);
        }
        
        public async Task<IEnumerable<AdvertisementDTO>> GetFilteredPagedAdvertisementsAsync(int pageNumber, int pageSize, string region, string category, string locationTown)
        {
            var ads = adRepo.GetPagedAds(pageNumber, pageSize).Where(x => x.Status != AdStatus.Rejected.ToString());

            if (isModerationEnabled)
            {
                ads = ads
                    .Where(x => x.Status == AdStatus.Approved.ToString());
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                ads = ads
                    .Where(x => x.Category == category);
            }
            if(!string.IsNullOrWhiteSpace(locationTown))
            {
                ads = ads
                    .Where(x => x.Pet.Location.Town == locationTown);                
               
            } else if (!string.IsNullOrWhiteSpace(region))
            {                
                ads = ads                        
                    .Where(x => x.Pet.Location.Region == region);                
            }            

            var result = await ads.ToListAsync();
            

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(result);
           
        }

        public async Task<IEnumerable<AdvertisementDTO>> GetPagedAdsByUserAsync(string userId, int pageNumber, int pageSize)
        {
            var ads = await adRepo.GetPagedListByUserAsync(userId, pageNumber, pageSize);

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(ads);
        }

        public async Task<IEnumerable<AdvertisementDTO>> GetAdsByUserAsync(string userId)
        {
            var ads = await adRepo.GetAdsByUserAsync(userId);

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(ads);
        }

        public async Task<AdvertisementDTO> UpdateAdvetrtisementAsync(AdvertisementDTO model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var adToUpdate = await adRepo.GetByIdAsync(model.Id);
           
            if(user == null)
            {
                logger.LogWarning($"Unauthorized user with id '{userId}' was trying to update ad with id '{model.Id}'");
                throw new UnauthorizedAccessException("Unauthorized access");
            }
            if (adToUpdate == null)
            {
                logger.LogWarning($"user with Id {model.Id} was trying to update advertisement that was not found. Id: {model.Id}");
                throw new NotFoundException($"Advertisement with Id {model.Id} was not found");
            }

            var roles = await userManager.GetRolesAsync(user);
            
            if(!roles.Contains("admin") || adToUpdate.UserId != userId)
            {
                logger.LogWarning($"user with Id '{userId} was trying to update ad with id {model.Id} having no permission to do that'");
                throw new ForbiddenAccessException("You don't have permission to update this advertisement");
            }

            Pet pet = mapper.Map<Pet>(model.Pet);

            Advertisement ad = new Advertisement
            {                                
                Description = model.Description,
                PublicationDate = DateTime.Now,
                Pet = pet,
                Category = model.Category,
                Images = mapper.Map<List<ImageDTO>, List<Image>>(model.Images),
            };

            var result = await adRepo.Update(model.Id, ad);

            return mapper.Map<AdvertisementDTO>(result);
        }

        public async Task<AdvertisementDTO> ApproveAdStatus(int id)
        {
            var ad = await adRepo.GetByIdAsync(id);
            
            if(ad == null)
            {
                logger.LogError($"Can't approve, Advertisement with id '{id}' was not found");
                throw new NotFoundException($"Can't approve, Advertisement with id '{id}' was not found");
            }

            var result = await adRepo.ChangeStatus(id, AdStatus.Approved.ToString());
            logger.LogInformation($"Ad's status with id '{id} has been changed to '{AdStatus.Approved.ToString()}'");

            return mapper.Map<AdvertisementDTO>(result);
        }


        public async Task<IEnumerable<AdvertisementDTO>> GetAllAdvertisementsAsync()
        {
            var ads = await adRepo.GetAll()
                .OrderByDescending(x => x.PublicationDate)
                .ToListAsync();

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(ads);
        }
        
    }
}
