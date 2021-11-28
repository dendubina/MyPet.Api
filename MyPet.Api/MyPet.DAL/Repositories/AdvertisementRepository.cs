using Microsoft.EntityFrameworkCore;
using MyPet.DAL.EF;
using MyPet.DAL.Entities;
using MyPet.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.DAL.Repositories
{
    public class AdvertisementRepository : BaseRepository<Advertisement>, IAdvertisementRepository
    {
       
        public AdvertisementRepository(AppDbContext context) : base(context)
        {
            
        }

        public async Task<IEnumerable<Advertisement>> GetAdsByUserAsync(string userId)
        {
            return await context.Advertisements
                .Where(x => x.UserId == userId)
                .Include(x => x.Images)
                .Include(x => x.Pet).ThenInclude(x => x.Location)
                .OrderByDescending(x => x.PublicationDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Advertisement>> GetPagedListAsync(int pageNumber, int pageSize)
        {
            return await context.Advertisements
                .Include(x => x.Images)
                .Include(x => x.Pet).ThenInclude(x => x.Location)
                .OrderByDescending(x => x.PublicationDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();            
        }

        public async Task<IEnumerable<Advertisement>> GetPagedListByUserAsync(string userId, int pageNumber, int pageSize)
        {
            return await context.Advertisements
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
            var ad = await context.Advertisements.FindAsync(id);

            if(entity.Images.Count() > 0)
            {
                ad.Images = entity.Images;
            }

            ad.Description = entity.Description;
            ad.Pet.Name = entity.Pet.Name;
            ad.Pet.Location.Town = entity.Pet.Location.Town;
            ad.Pet.Location.Street = entity.Pet.Location.Street;

            context.SaveChanges();

            return ad;
        }
    }
}
