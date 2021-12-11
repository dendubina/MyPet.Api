using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyPet.BLL.DTO;
using MyPet.BLL.Interfaces;
using MyPet.DAL.Entities;
using MyPet.DAL.Interfaces;
using MyPet.DAL.Repositories;
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

        public AdvertisementService(IAdvertisementRepository adRepo, IMapper mapper)
        {
            this.adRepo = adRepo;
            this.mapper = mapper;
        }

        public async Task AddAdvertisementAsync(AdvertisementDTO model)
        {
            Pet pet = mapper.Map<Pet>(model.Pet);            

            Advertisement ad = new Advertisement
            {
                UserId = model.UserId,
                UserName = model.UserName,
                Description = model.Description,
                Category = model.Category,
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

        public async Task<IEnumerable<AdvertisementDTO>> GetAllAdvertisementsAsync()
        {
            var ads = await adRepo.GetAll()
               // .OrderByDescending(x => x.PublicationDate)
                .ToListAsync();            

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(ads);           
        }       

        public async Task<AdvertisementDTO> DeleteAdvertisementAsync(int id)
        {
            var ad = await adRepo.DeleteAsync(id);

            return mapper.Map<AdvertisementDTO>(ad);
        }

        public async Task<IEnumerable<AdvertisementDTO>> GetFilteredPagedAdvertisementsAsync(int pageNumber, int pageSize, string region, string category, string locationTown)
        {
            var ads = adRepo.GetPagedAds(pageNumber, pageSize);


            if(category != "all")
            {
                ads = ads
                    .Where(x => x.Category == category);
            }

            if(locationTown != "all")
            {
                ads = ads
                    .Where(x => x.Pet.Location.Town == locationTown);
                
               
            } else if (region != "all")
            {
                
                    ads = ads
                        
                        .Where(x => x.Pet.Location.Region == region);
                
                   
                
            }
            ads = ads
                .OrderByDescending(x => x.PublicationDate);

            var result = await ads.ToListAsync();

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(result);


            /*if(locationTown != "all")
            {
                if(category != "all")
                {                    
                    return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(await adRepo.GetPagedListByTownAndCategoryAsync(pageNumber, pageSize, locationTown, category));
                }
                
                return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(await adRepo.GetPagedListByTownAsync(pageNumber, pageSize, locationTown));

            }else if (region != "all")
            {
                if (category != "all")
                {
                    return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(await adRepo.GetPagedListByRegionAndCategoryAsync(pageNumber, pageSize, region, category));
                }
                return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(await adRepo.GetPagedListByRegionAsync(pageNumber, pageSize, region));

            }else if (category != "all")
            {
                return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(await adRepo.GetPagedListByCategoryAsync(pageNumber, pageSize, category));
            }            

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(await adRepo.GetPagedListAsync(pageNumber, pageSize, category, locationTown));*/
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

        public async Task<AdvertisementDTO> UpdateAdvetrtisementAsync(AdvertisementDTO model)
        {
            Pet pet = mapper.Map<Pet>(model.Pet);

            Advertisement ad = new Advertisement
            {
                UserId = model.UserId,
                UserName = model.UserName,
                Description = model.Description,
                PublicationDate = DateTime.Now,
                Pet = pet,
                Category = model.Category,
                Images = mapper.Map<List<ImageDTO>, List<Image>>(model.Images),
            };

            var result = await adRepo.Update(model.Id, ad);

            return mapper.Map<AdvertisementDTO>(result);
        }
    }
}
