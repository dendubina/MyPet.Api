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

        public AdvertisementService(IAdvertisementRepository adRepo )
        {
            this.adRepo = adRepo;
        }

        public void AddAdvertisement(AdvertisementDTO model)
        {
            var ad = new Advertisement()
            {
                UserId = model.UserId,
                UserName = model.UserName,
                PetName = model.PetName,
                Description = model.Description,
                PublicationDate = DateTime.Now,
                Images = new List<Image>(),
            };

            foreach(var image in model.Images)
            {
                var img = new Image
                {
                    Path = image.Path,
                    Size = image.Size,
                };
                ad.Images.Add(img);
            }

            adRepo.Add(ad);
        }
        
        public AdvertisementDTO GetAdvertisementById(int id)
        {
            var ad = adRepo.GetById(id);

            AdvertisementDTO adDTO = new AdvertisementDTO
            {
                Id = ad.Result.Id,
                UserId = ad.Result.UserId,
                UserName = ad.Result.UserName,
                PetName = ad.Result.PetName,
                Description = ad.Result.Description,
                Images = new List<ImageDTO>(),
            };

            foreach(var img in ad.Result.Images)
            {
                adDTO.Images.Add(new ImageDTO
                {
                    Path = img.Path,
                    Size = img.Size,
                });
            }

            return adDTO;
        }

        public IEnumerable<AdvertisementDTO> GetAllAdvertisements()
        {
            var ads = adRepo.GetAll().Result;
            List<AdvertisementDTO> dtoList = new List<AdvertisementDTO>();

            foreach(var ad in ads)
            {
                var dto = new AdvertisementDTO
                {
                    Id = ad.Id,
                    Description = ad.Description,
                    PetName = ad.PetName,
                    PublicationDate = ad.PublicationDate,
                    UserId = ad.UserId,
                    UserName = ad.UserName,
                    Images = new List<ImageDTO>(),
                };

                foreach(var img in ad.Images)
                {
                    dto.Images.Add(new ImageDTO {
                        Path = img.Path,
                        Size = img.Size,
                    });
                }

                dtoList.Add(dto);
            }

            return dtoList;
        }

        public AdvertisementDTO UpdateAdvertisement(int id, AdvertisementDTO advertisementDTO)
        {
            throw new NotImplementedException();
        }
        public void DeleteAdvertisement(int id)
        {
            throw new NotImplementedException();
        }
    }
}
