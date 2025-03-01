using Core_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Inetrfaces.Repositries
{
    public interface IUserFollowersRepository
    {
        public Task InsertAsync(UserFollowers entity);
        public void Update(UserFollowers entity);
        public void Delete(UserFollowers entity);

        public Task<UserFollowers> GetAsync(params string[] key);
        public Task<IEnumerable< UserFollowers>> GetAllAsync();
        public Task<int> CompleteAsync();
        public  Task<IEnumerable<UserFollowers>> GetAllWithSpecAsync(ISpecification<UserFollowers> specs);
          

    }
}
