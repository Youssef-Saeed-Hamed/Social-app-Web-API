using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Entities
{
    public class UserFollowers
    {
        public User Follower { get; set; }
        public string FollowerId {  get; set; }

        public User Following { get; set; }
        public string FollowingId { get; set; }

        public bool IsFollowing { get; set; }
        public DateTime DateTime { get; set; }

    }
}
