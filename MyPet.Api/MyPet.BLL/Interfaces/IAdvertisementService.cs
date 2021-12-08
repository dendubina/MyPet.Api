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
        Task<AdvertisementDTO> DeleteAdvertisementAsync(int id);
        Task<IEnumerable<AdvertisementDTO>> GetFilteredPagedAdvertisementsAsync(int pageNumber, int pageSize, string region = "all", string category = "all", string locationTown = "all");
        Task<IEnumerable<AdvertisementDTO>> GetPagedAdsByUserAsync(string userId, int pageNumber, int pageSize);
        Task<IEnumerable<AdvertisementDTO>> GetAdsByUserAsync(string userId);
        Task<AdvertisementDTO> UpdateAdvetrtisementAsync(AdvertisementDTO model);
    }
}
