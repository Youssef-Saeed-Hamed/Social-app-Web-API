using Core_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Inetrfaces.Repositries
{
    public interface IGenericRepositry<TEntity , Tkey> where TEntity : BaseEntity<Tkey>
    {
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public Task<IEnumerable<TEntity>> GetAllWithSpecAsync(ISpecification<TEntity> specs);
        public Task<TEntity> GetAsync(Tkey Id);
        public Task<TEntity> GetWithSpecAsync(ISpecification<TEntity> specs);
        public Task InsertAsync(TEntity entity);
        public void Update(TEntity entity);
        public void Delete(TEntity entity);
    }
}
