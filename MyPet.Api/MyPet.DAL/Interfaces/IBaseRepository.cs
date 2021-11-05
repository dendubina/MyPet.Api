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
        Task<TEntity> GetById(int id);
        void Delete(int id);
        Task<IEnumerable<TEntity>> GetAll();
        TEntity Update(int id, TEntity entity);
    }
}
