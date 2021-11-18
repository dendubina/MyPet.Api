using Microsoft.EntityFrameworkCore;
using MyPet.DAL.EF;
using MyPet.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.DAL.Repositories
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class
    {
        protected readonly DbSet<TEntity> settedEntity;
        protected readonly AppDbContext context;

        public BaseRepository(AppDbContext context)
        {
            this.context = context;
            settedEntity = this.context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await settedEntity.AddAsync(entity);
            context.SaveChanges();
        }

        public async Task<TEntity> DeleteAsync(int id)
        {
            TEntity entity = await settedEntity.FindAsync(id);

            settedEntity.Remove(entity);
            context.SaveChanges();

            return entity;
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {            
            return await settedEntity.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await context.Set<TEntity>().ToListAsync();
        }

        public abstract TEntity Update(int id, TEntity entity);

    }
}
