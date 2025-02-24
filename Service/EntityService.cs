using OnlineShoppingSystem_Main.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class EntityService<TEntity> : IEntityService<TEntity> where TEntity : class
    {
        private readonly Swd392OssContext _dbContext;

        public Task<bool> ExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(params object[] keys)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetByKeyAsync(params object[] keys)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> GetByParamAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveAsync(TEntity entityToSave)
        {
            throw new NotImplementedException();
        }
    }
}
