using MyPet.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Interfaces
{
    public interface IAdvertisementService
    {
        void AddAdvertisement(AdvertisementDTO model);
        IEnumerable<AdvertisementDTO> GetAllAdvertisements();
        AdvertisementDTO GetAdvertisementById(int id);
        AdvertisementDTO UpdateAdvertisement(int id, AdvertisementDTO advertisementDTO);
        void DeleteAdvertisement(int id);
    }
}
