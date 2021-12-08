using MyPet.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.DAL.Interfaces
{
    public interface IAdvertisementRepository : IBaseRepository<Advertisement>
    {
        Task<IEnumerable<Advertisement>> GetPagedListAsync(int pageNumber, int pageSize, string category, string locationTown);
        Task<IEnumerable<Advertisement>> GetAdsByUserAsync(string userId);
        Task<IEnumerable<Advertisement>> GetPagedListByUserAsync(string userId, int pageNumber, int pageSize);

        Task<IEnumerable<Advertisement>> GetPagedListByRegionAsync(int pageNumber, int pageSize, string region);
        Task<IEnumerable<Advertisement>> GetPagedListByTownAndCategoryAsync(int pageNumber, int pageSize, string locationTown, string category);
        Task<IEnumerable<Advertisement>> GetPagedListByRegionAndCategoryAsync(int pageNumber, int pageSize, string region, string category);
        Task<IEnumerable<Advertisement>> GetPagedListByTownAsync(int pageNumber, int pageSize, string town);
        Task<IEnumerable<Advertisement>> GetPagedListByCategoryAsync(int pageNumber, int pageSize, string category);
    }
}
