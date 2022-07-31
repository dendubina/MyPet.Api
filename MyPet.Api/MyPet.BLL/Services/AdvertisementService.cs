using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyPet.BLL.Constants;
using MyPet.BLL.DTO;
using MyPet.BLL.Exceptions;
using MyPet.BLL.Interfaces;
using MyPet.BLL.Models.Ads;
using MyPet.DAL.Entities;
using MyPet.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IConfiguration config;


        public AdvertisementService(IAdvertisementRepository adRepo, IMapper mapper, IConfiguration config, ILogger<AdvertisementService> logger, UserManager<IdentityUser> userManager, IWebHostEnvironment env)
        {
            this.adRepo = adRepo;
            this.mapper = mapper;
            this.logger = logger;
            this.userManager = userManager;
            this.config = config;
            webHostEnvironment = env;
            isModerationEnabled = bool.Parse(config["EnableAdsPreModeration"]);
        }

        public async Task<AdvertisementDTO> AddAdvertisementAsync(AdvertisementModel model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if(user == null)
            {
                logger.LogError($"user with id '{userId}' not found trying to add advertisement");
                throw new UnauthorizedAccessException($"user with id '{userId} not found'");
            }

            var pet = new Pet
            {
                Name = model.PetName,
                Location = mapper.Map<Location>(model)
            };
            var ad = new Advertisement
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserEmail = user.Email,
                PublicationDate = DateTime.Now,
                Description = model.Description,
                Category = model.Category,
                Pet = pet,
                Images = new List<Image>(),
            };

            ad.Images.Add(new Image
            {
                Size = model.Image.Length,
                Path = await AddImageGetPath(model.Image),
            });

            ad.Status = isModerationEnabled ? AdStatuses.OnModeration : AdStatuses.Approved;             

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

            if (!isModerationEnabled)
            {
                return mapper.Map<AdvertisementDTO>(ad);
            }

            if (ad.Status != AdStatuses.OnModeration && ad.Status != AdStatuses.Rejected)
            {
                return mapper.Map<AdvertisementDTO>(ad);
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user != null && await userManager.IsInRoleAsync(user, "admin"))
            {
                return mapper.Map<AdvertisementDTO>(ad);
            }

            logger.LogWarning($"Attempt to get advertisement with id '{id}' having no permission");
            throw new ForbiddenAccessException("You don't have permission to get this advertisement");
        }        

        public async Task<AdvertisementDTO> DeleteAdvertisementAsync(int id, string userId)
        {
            var adToDelete = await adRepo.GetByIdAsync(id);
            var user = await userManager.FindByIdAsync(userId);

            if (adToDelete == null)
            {
                logger.LogWarning($"user with Id {userId} was trying to delete advertisement that was not found. Id: {id}");
                throw new NotFoundException($"Advertisement with Id {id} was not found to delete");
            }

            if (user == null)
            {
                logger.LogWarning($"Unauthorized user with id '{userId}' was trying to delete ad with id '{id}'");
                throw new UnauthorizedAccessException("Unauthorized access");
            }            

            if (adToDelete.UserId != userId && !await userManager.IsInRoleAsync(user, "admin"))
            {
                logger.LogWarning($"user with Id '{userId} was trying to delete ad with id {id} having no permission to delete advertisement'");
                throw new ForbiddenAccessException("You don't have permission to delete this advertisement");
            }

            var images = adToDelete.Images;
            var result = await adRepo.DeleteAsync(id);

            if(result != null)
            {
                foreach(var img in images)
                {
                    DeleteAdsImage(img.Path);
                }
            }
               

            return mapper.Map<AdvertisementDTO>(result);
        }
        
        public async Task<IEnumerable<AdvertisementDTO>> GetFilteredPagedAdvertisementsAsync(int pageNumber, int pageSize, string region, string category, string locationTown)
        {

            var ads = adRepo.GetAll();

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
            
            ads = ads
                .Include(x => x.Images)
                .Include(x => x.Pet).ThenInclude(x => x.Location)
                .OrderByDescending(x => x.PublicationDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking();

            var result = await ads.ToListAsync();

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(result);           
        }

        public async Task<IEnumerable<AdvertisementDTO>> GetPagedAdsByUserAsync(string userId, int pageNumber, int pageSize)
        {
            var ads = await adRepo.GetPagedListByUserAsync(userId, pageNumber, pageSize);

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(ads);
        }               

        public async Task<AdvertisementDTO> UpdateAdvetrtisementAsync(AdvertisementModel model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var adToUpdate = await adRepo.GetByIdAsync(model.AdId);
           
            if(user == null)
            {
                logger.LogWarning($"Unauthorized user with id '{userId}' was trying to update ad with id '{model.AdId}'");
                throw new UnauthorizedAccessException("Unauthorized access");
            }
            if (adToUpdate == null)
            {
                logger.LogWarning($"user with Id {model.AdId} was trying to update advertisement that was not found. Id: {model.AdId}");
                throw new NotFoundException($"Advertisement with Id {model.AdId} was not found");
            }            
            if(adToUpdate.UserId != userId && !await userManager.IsInRoleAsync(user, "admin"))
            {
                logger.LogWarning($"user with Id '{userId} was trying to update ad with id {adToUpdate.Id} having no permission to do that'");
                throw new ForbiddenAccessException("You don't have permission to update this advertisement");
            }

            var pet = new Pet
            {
                Name = model.PetName,
                Location = mapper.Map<Location>(model)
            };

            var ad = new Advertisement
            {                
                Description = model.Description,
                Category = model.Category,
                PublicationDate = adToUpdate.PublicationDate,
                Pet = pet,
                Images = new List<Image>(),
            };

            if(model.Image != null)
            {
                foreach(var img in adToUpdate.Images)
                {
                    DeleteAdsImage(img.Path);
                }                

                ad.Images.Add(new Image
                {
                    Size = model.Image.Length,
                    Path = await AddImageGetPath(model.Image),
                });
            }

            if (isModerationEnabled)
            {
                ad.Status = AdStatuses.OnModeration;
            }

            var result = await adRepo.Update(model.AdId, ad);

            return mapper.Map<AdvertisementDTO>(result);
        }

        public async Task<AdvertisementDTO> ChangeAdStatusAsync(int adId, string status)
        {
            var ad = await adRepo.GetByIdAsync(adId);

            if (ad == null)
            {
                logger.LogError($"Can't change status, Advertisement with id '{adId}' was not found");
                throw new NotFoundException($"Can't change status, Advertisement with id '{adId}' was not found");
            }

            if (status == AdStatuses.Approved || status == AdStatuses.Rejected || status == AdStatuses.OnModeration)
            {
                var result = await adRepo.ChangeStatus(adId, status);
                logger.LogInformation($"Ad's status with id '{adId} has been changed to '{status}'");
                return mapper.Map<AdvertisementDTO>(result);
            }

            throw new ValidationException("adStatus is invalid", new Dictionary<string, string[]> { { "adStatus", new [] { "adStatus is invalid" } } });
        }

        public async Task<IEnumerable<AdvertisementDTO>> GetAdsOnModerationAsync()
        {
            var ads = await adRepo.GetAll()
                .OrderByDescending(x => x.PublicationDate)
                .Where(x => x.Status == AdStatuses.OnModeration)
                .ToArrayAsync();

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(ads);
        }

        private async Task<string> AddImageGetPath(IFormFile image)
        {
            string imagesFolder = config["ImagesFolder"];
            string folderToSave = webHostEnvironment.WebRootPath + imagesFolder;

            string filename = Path.GetRandomFileName();

            filename = Path.GetFileNameWithoutExtension(filename);
            filename += Path.GetExtension(image.FileName);

            string fullpath = Path.Combine(folderToSave, filename);

            await using (var stream = File.Create(fullpath))
            {
                await image.CopyToAsync(stream);
            }

            return imagesFolder + filename;
        }

        private void DeleteAdsImage(string path)
        {
            string[] subs = path.Split("/");            
            string pathToImg = webHostEnvironment.WebRootPath + config["ImagesFolder"] + subs[2];
            File.Delete(pathToImg);
        }
    }
}
