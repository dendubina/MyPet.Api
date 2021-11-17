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
        Task AddAdvertisementAsync(AdvertisementDTO model);
        Task<IEnumerable<AdvertisementDTO>> GetAllAdvertisementsAsync();
        Task<AdvertisementDTO> GetAdvertisementByIdAsync(int id);
        AdvertisementDTO UpdateAdvertisement(int id, AdvertisementDTO advertisementDTO);
        void DeleteAdvertisement(int id);
        Task<IEnumerable<AdvertisementDTO>> GetPagedAdvertisementsAsync(int pageNumber, int pageSize);
    }
}
