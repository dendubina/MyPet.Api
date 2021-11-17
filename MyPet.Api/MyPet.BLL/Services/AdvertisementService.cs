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
            var ad = await adRepo.GetById(id);           

            return mapper.Map<AdvertisementDTO>(ad);
        }

        public async Task<IEnumerable<AdvertisementDTO>> GetAllAdvertisementsAsync()
        {
            var ads = await adRepo.GetAll();
            var result = ads.OrderByDescending(x => x.PublicationDate);

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(result);           
        }

        public AdvertisementDTO UpdateAdvertisement(int id, AdvertisementDTO advertisementDTO)
        {
            throw new NotImplementedException();
        }

        public void DeleteAdvertisement(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AdvertisementDTO>> GetPagedAdvertisementsAsync(int pageNumber, int pageSize)
        {
            var ads = await adRepo.GetPagedListAsync(pageNumber, pageSize);

            return mapper.Map<IEnumerable<Advertisement>, IEnumerable<AdvertisementDTO>>(ads);
        }
    }
}
