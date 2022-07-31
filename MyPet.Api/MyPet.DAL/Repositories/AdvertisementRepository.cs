using Microsoft.EntityFrameworkCore;
using MyPet.DAL.EF;
using MyPet.DAL.Entities;
using MyPet.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.DAL.Repositories
{
    public class AdvertisementRepository : BaseRepository<Advertisement>, IAdvertisementRepository
    {
       
        public AdvertisementRepository(AppDbContext context) : base(context)
        {
            
        }       

        public async Task<IEnumerable<Advertisement>> GetPagedListByUserAsync(string userId, int pageNumber, int pageSize)
        {
            return await DbContext.Advertisements
                .Where(x => x.UserId == userId)
                .Include(x => x.Images)
                .Include(x => x.Pet).ThenInclude(x => x.Location)
                .OrderByDescending(x => x.PublicationDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Advertisement> Update(int id, Advertisement entity)
        {
            var ad = await DbContext.Advertisements.FindAsync(id);

            if(entity.Images.Any())
            {
                ad.Images.Clear();
                ad.Images = entity.Images;
            }

            ad.Description = entity.Description;
            ad.Category = entity.Category;
            ad.Pet.Name = entity.Pet.Name;
            ad.Status = entity.Status;
            ad.PublicationDate = entity.PublicationDate;
            ad.Pet.Location.Town = entity.Pet.Location.Town;
            ad.Pet.Location.Street = entity.Pet.Location.Street;
            ad.Pet.Location.House = entity.Pet.Location.House;
            ad.Pet.Location.Region = entity.Pet.Location.Region;            

            await DbContext.SaveChangesAsync();
            return ad;
        }

        public async Task<Advertisement> ChangeStatus(int id, string status)
        {
            var ad = await DbContext.Advertisements.FindAsync(id);

            ad.Status = status;
            await DbContext.SaveChangesAsync();

            return ad;
        }
    }
}
