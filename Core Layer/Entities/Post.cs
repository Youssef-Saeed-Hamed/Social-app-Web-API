using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Entities
{
    public class Post : BaseEntity<string>
    {
        public string Content { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public ICollection<LikePost> LikePosts { get; set; }


        public DateTime DateTime { get; set; }
        public string? imagePath {  get; set; }
    }
}
