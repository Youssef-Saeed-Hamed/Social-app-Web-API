using Core_Layer.Entities;
using Core_Layer.Inetrfaces;
using Core_Layer.Inetrfaces.Repositries;
using Microsoft.EntityFrameworkCore;
using Repository_Layer.Context;
using Repository_Layer.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Repositries
{
    public class GenericRepository<TEntity, TKey> : IGenericRepositry<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {

        private readonly DataContext _context;

        public GenericRepository(DataContext contex)
        {
            _context = contex;
        }

        
        public void Delete(TEntity entity)
            => _context.Set<TEntity>().Remove(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync()
            => await _context.Set<TEntity>().ToListAsync();

        public async Task<IEnumerable<TEntity>> GetAllWithSpecAsync(ISpecification<TEntity> specs)
            => await ApplySpecification(specs).ToListAsync();
        

        private IQueryable<TEntity>ApplySpecification(ISpecification<TEntity> specs)
            => SpecificationEvaluator<TEntity, TKey>.BuildQuery(_context.Set<TEntity>(), specs);
        

        public async Task<TEntity> GetAsync(TKey Id)
            => (await _context.Set<TEntity>().FindAsync(Id))!;

        public Task<TEntity> GetWithSpecAsync(ISpecification<TEntity> specs)
            => (ApplySpecification(specs).FirstOrDefaultAsync())!;

        public async Task InsertAsync(TEntity entity)
            => await _context.Set<TEntity>().AddAsync(entity);

        public void Update(TEntity entity)
            => _context.Set<TEntity>().Update(entity);
    }
}
