using Core_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Inetrfaces.Repositries
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        public IGenericRepositry<TEntity, Tkey> Repositry<TEntity, Tkey>() where TEntity : BaseEntity<Tkey>;
        public Task<int> CompleteAsync();
    }
}
