using Microsoft.EntityFrameworkCore;
using MyPet.DAL.EF;
using MyPet.DAL.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.DAL.Repositories
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class
    {
        protected readonly DbSet<TEntity> SettedEntity;
        protected readonly AppDbContext DbContext;

        protected BaseRepository(AppDbContext context)
        {
            DbContext = context;
            SettedEntity = this.DbContext.Set<TEntity>();
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await SettedEntity.AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<TEntity> DeleteAsync(int id)
        {
            var entity = await SettedEntity.FindAsync(id);

            SettedEntity.Remove(entity);
            await DbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {            
            return await SettedEntity.FindAsync(id);
        }

        public IQueryable<TEntity> GetAll()
        {
            return DbContext.Set<TEntity>();
        }

        public abstract Task<TEntity> Update(int id, TEntity entity);

    }
}
