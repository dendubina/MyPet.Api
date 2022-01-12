using MyPet.BLL.DTO;
using MyPet.BLL.Models.Ads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Interfaces
{
    public interface IAdvertisementService
    {
        Task<AdvertisementDTO> AddAdvertisementAsync(AdvertisementModel model, string userId);        
        Task<AdvertisementDTO> GetAdvertisementByIdAsync(int id, string userId);        
        Task<AdvertisementDTO> DeleteAdvertisementAsync(int id, string userId);
        Task<IEnumerable<AdvertisementDTO>> GetFilteredPagedAdvertisementsAsync(int pageNumber, int pageSize, string region, string category, string locationTown);
        Task<IEnumerable<AdvertisementDTO>> GetPagedAdsByUserAsync(string userId, int pageNumber, int pageSize);        
        Task<AdvertisementDTO> UpdateAdvetrtisementAsync(AdvertisementModel model, string userId);
        Task<AdvertisementDTO> ChangeAdStatusAsync(int AdId, string status);
        Task<IEnumerable<AdvertisementDTO>> GetAdsOnModerationAsync();


      //  Task<IEnumerable<AdvertisementDTO>> GetAllAdvertisementsAsync();
    }
}
