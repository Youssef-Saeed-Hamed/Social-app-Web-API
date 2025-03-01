using Core_Layer.Entities;
using Core_Layer.Inetrfaces;
using Core_Layer.Inetrfaces.Repositries;
using Microsoft.EntityFrameworkCore;
using Repository_Layer.Context;
using Repository_Layer.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Repositries
{
    public class UserFollowersRepository : IUserFollowersRepository
    {

        private readonly DataContext _context;

        public UserFollowersRepository(DataContext context)
        {
            _context = context;
        }

        public void Delete(UserFollowers entity)
            => _context.UserFollowers.Remove(entity);

        public async Task<UserFollowers> GetAsync(params string[] key)
            => (await _context.UserFollowers.FindAsync(key))!;

        private static IQueryable<UserFollowers> BuildQuery(IQueryable<UserFollowers> inputQuery, ISpecification<UserFollowers> specs)
        {
            var query = inputQuery;
            if (specs.Criteria is not null)
                query = query.Where(specs.Criteria);

            foreach (var item in specs.Includes)
            {
                query = query.Include(item);
            }

            return query;
        }

        private IQueryable<UserFollowers> ApplySpecification(ISpecification<UserFollowers> specs)
           => BuildQuery(_context.Set<UserFollowers>(), specs);

        public async Task<IEnumerable<UserFollowers>> GetAllWithSpecAsync(ISpecification<UserFollowers> specs)
            => await ApplySpecification(specs).ToListAsync();


        public async Task InsertAsync(UserFollowers entity)
            => await _context.UserFollowers.AddAsync(entity);
        

        public void Update(UserFollowers entity)
            => _context.UserFollowers.Update(entity);

       

        public async Task<int> CompleteAsync()
            => await _context.SaveChangesAsync();

        public async Task<IEnumerable<UserFollowers>> GetAllAsync()
            => await _context.UserFollowers.ToListAsync();
            
        
    }
}
