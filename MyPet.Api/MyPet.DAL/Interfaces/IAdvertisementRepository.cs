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

        IQueryable<Advertisement> GetPagedAds(int pageNumber, int pageSize);
        
    }
}
