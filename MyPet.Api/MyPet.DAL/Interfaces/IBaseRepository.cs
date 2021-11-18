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
        Task AddAsync(TEntity entity);
        Task<TEntity> GetByIdAsync(int id);
        Task<TEntity> DeleteAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        TEntity Update(int id, TEntity entity);
    }
}
