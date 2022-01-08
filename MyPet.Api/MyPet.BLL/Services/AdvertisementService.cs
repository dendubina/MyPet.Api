using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyPet.BLL.Constants;
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


        public AdvertisementService(IAdvertisementRepository adRepo, IMapper mapper, IConfiguration config, ILogger<AdvertisementService> logger, UserManager<IdentityUser> userManager)
        {
            this.adRepo = adRepo;
            this.mapper = mapper;
            this.logger = logger;
            this.userManager = userManager;
            isModerationEnabled = bool.Parse(config["EnableAdsPreModeration"]);
        }

        public async Task<AdvertisementDTO> AddAdvertisementAsync(AdvertisementDTO model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if(user == null)
            {
                logger.LogError($"user with id '{userId}' not found trying to add advertisement");
                throw new UnauthorizedAccessException($"user with id '{userId} not found'");
            }


            Pet pet = mapper.Map<Pet>(model.Pet);           
            Advertisement ad = new Advertisement
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
                Description = model.Description,
                Category = model.Category,               
                PublicationDate = DateTime.Now,
                Pet = pet,
                Images = mapper.Map<List<ImageDTO>,List<Image>>(model.Images),
            };

            if (isModerationEnabled)
                ad.Status = AdStatuses.OnModeration;
            else
                ad.Status = AdStatuses.Approved;

            var result = await adRepo.AddAsync(ad);

            logger.LogInformation($"user with id '{result.UserId}' added advertisement with id '{result.Id}'");
            return mapper.Map<AdvertisementDTO>(result);
        }        

        public async Task<AdvertisementDTO> GetAdvertisementByIdAsync(int id, string userId)
        {
            var ad = await adRepo.GetByIdAsync(id);

            if (ad == null)
            {
                logger.LogWarning($"Attempt to get advertisement with id '{id}' that not found");
                throw new NotFoundException($"Advertisement with id '{id}' was not found");
            }

            if (isModerationEnabled)
            {
                if (ad.Status == AdStatuses.OnModeration || ad.Status == AdStatuses.Rejected)
                {
                    var user = await userManager.FindByIdAsync(userId);

                    if(user != null && await userManager.IsInRoleAsync(user, "admin"))
                    {
                        return mapper.Map<AdvertisementDTO>(ad);
                    }
                    else
                    {
                        logger.LogWarning($"Attempt to get advertisement with id '{id}' having no permission");
                        throw new ForbiddenAccessException("You don't have permission to get this advertisement");
                    }
                }
            }

            return mapper.Map<AdvertisementDTO>(ad);
        }        

        public async Task<AdvertisementDTO> DeleteAdvertisementAsync(int id, string userId)
        {
            var adTodelete = await adRepo.GetByIdAsync(id);
            var user = await userManager.FindByIdAsync(userId);

            if (adTodelete == null)
            {
                logger.LogWarning($"user with Id {userId} was trying to delete advertisement that was not found. Id: {id}");
                throw new NotFoundException($"Advertisement with Id {id} was not found to delete");
            }

            if (user == null)
            {
                logger.LogWarning($"Unauthorized user with id '{userId}' was trying to delete ad with id '{id}'");
                throw new UnauthorizedAccessException("Unauthorized access");
            }            

            if (adTodelete.UserId != userId && !await userManager.IsInRoleAsync(user, "admin"))
            {
                logger.LogWarning($"user with Id '{userId} was trying to delete ad with id {id} having no permission to delete advertisement'");
                throw new ForbiddenAccessException("You don't have permission to delete this advertisement");
            }

            var result = await adRepo.DeleteAsync(id);

            return mapper.Map<AdvertisementDTO>(result);
        }
        
        public async Task<IEnumerable<AdvertisementDTO>> GetFilteredPagedAdvertisementsAsync(int pageNumber, int pageSize, string region, string category, string locationTown)
        {
            var ads = adRepo.GetPagedAds(pageNumber, pageSize);

            if (isModerationEnabled)
            {
                ads = ads                    
                    .Where(x => x.Status == AdStatuses.Approved);
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
            if(adToUpdate.UserId != userId && !await userManager.IsInRoleAsync(user, "admin"))
            {
                logger.LogWarning($"user with Id '{userId} was trying to update ad with id {adToUpdate.Id} having no permission to do that'");
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

            if (isModerationEnabled)            
                ad.Status = AdStatuses.OnModeration;
            

            var result = await adRepo.Update(model.Id, ad);

            return mapper.Map<AdvertisementDTO>(result);
        }

        public async Task<AdvertisementDTO> ChangeAdStatusAsync(int AdId, string status)
        {
            var ad = await adRepo.GetByIdAsync(AdId);

            if (ad == null)
            {
                logger.LogError($"Can't change status, Advertisement with id '{AdId}' was not found");
                throw new NotFoundException($"Can't change status, Advertisement with id '{AdId}' was not found");
            }

            if (status == AdStatuses.Approved || status == AdStatuses.Rejected || status == AdStatuses.OnModeration)
            {
                var result = await adRepo.ChangeStatus(AdId, status);
                logger.LogInformation($"Ad's status with id '{AdId} has been changed to '{status}'");
                return mapper.Map<AdvertisementDTO>(result);
            }

            throw new ValidationException("adStatus is invalid", new Dictionary<string, string[]> { { "adStatus", new string[] { "adStatus is invalid" } } });

        }

        public async Task<IEnumerable<AdvertisementDTO>> GetAdsOnModerationAsync()
        {
            var ads = await adRepo.GetAll()
                .OrderByDescending(x => x.PublicationDate)
                .Where(x => x.Status == AdStatuses.OnModeration)
                .ToListAsync();

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(ads);
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
