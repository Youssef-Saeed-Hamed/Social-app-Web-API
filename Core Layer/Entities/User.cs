using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Entities
{
    public class User : BaseEntity<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime BirthDate { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public ICollection<UserFollowers> Followers { get; set; }
        public ICollection<UserFollowers> Followings { get; set; }
        public ICollection<Notification> MyNotifications { get; set; }
        public ICollection<Notification> SendingNotifications { get; set; }
        public bool IsBlind { get; set; }





    }
}
