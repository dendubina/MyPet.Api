using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.DAL.Interfaces
{
    public interface IBaseRepository<TEntity>
         where TEntity : class
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> GetByIdAsync(int id);
        Task<TEntity> DeleteAsync(int id);
        IQueryable<TEntity> GetAll();
        Task<TEntity> Update(int id, TEntity entity);
    }
}
