using AutoMapper;
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
            var ads = await adRepo.GetAllAsync();
            var result = ads.OrderByDescending(x => x.PublicationDate);

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(result);           
        }       

        public async Task<AdvertisementDTO> DeleteAdvertisementAsync(int id)
        {
            var ad = await adRepo.DeleteAsync(id);

            return mapper.Map<AdvertisementDTO>(ad);
        }

        public async Task<IEnumerable<AdvertisementDTO>> GetPagedAdvertisementsAsync(int pageNumber, int pageSize)
        {
            var ads = await adRepo.GetPagedListAsync(pageNumber, pageSize);

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(ads);
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
                Images = mapper.Map<List<ImageDTO>, List<Image>>(model.Images),
            };

            var result = await adRepo.Update(model.Id, ad);

            return mapper.Map<AdvertisementDTO>(result);
        }
    }
}
