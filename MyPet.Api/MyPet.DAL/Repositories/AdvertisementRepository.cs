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
        public override Advertisement Update(int id, Advertisement entity)
        {
            throw new NotImplementedException();
        }
    }
}
