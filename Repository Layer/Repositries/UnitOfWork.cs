using Core_Layer.Entities;
using Core_Layer.Inetrfaces.Repositries;
using Repository_Layer.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Repositries
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly Hashtable _repos;

        public UnitOfWork(DataContext context)
        {
            _context = context;
            _repos = new Hashtable();
        }

        public async Task<int> CompleteAsync()
            => await _context.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await _context.DisposeAsync();
        public IGenericRepositry<TEntity, Tkey> Repositry<TEntity, Tkey>() where TEntity : BaseEntity<Tkey>
        {
            var TypeName = typeof(TEntity).Name;
            if (_repos.ContainsKey(TypeName))
                return (_repos[TypeName] as GenericRepository<TEntity, Tkey>)!;
            var repo = new GenericRepository<TEntity, Tkey>(_context);
            _repos.Add(TypeName, repo);
            return repo;
        }
    }
}
