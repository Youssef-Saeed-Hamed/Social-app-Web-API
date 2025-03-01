using Core_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Specifications
{
    public class UserFollowersSpecification : BaseSpecification<UserFollowers>
    {
        public UserFollowersSpecification(string? FollowerId , string? FollowingId   ) : base(u => (
           (u.FollowerId == FollowerId || u.FollowingId == FollowingId)
        ))
        {
            Includes.Add(u => u.Follower );
            Includes.Add(u => u.Following );
        }
    }
}
