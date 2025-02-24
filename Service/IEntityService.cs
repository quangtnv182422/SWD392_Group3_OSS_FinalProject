using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IEntityService<TEntity>
    {
        Task<TEntity> GetByIdAsync(int id);

        Task<TEntity> GetByKeyAsync(params object[] keys);

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> GetByParamAsync();

        Task<int> SaveAsync(TEntity entityToSave);

        Task<bool> ExistsAsync(int id);

        Task<bool> ExistsAsync(params object[] keys);
    }
}

