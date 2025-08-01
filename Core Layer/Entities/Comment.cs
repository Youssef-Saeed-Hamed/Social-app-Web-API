using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Entities
{
    public class Comment : BaseEntity<string>
    {
        public string Content { get; set; }
        public List<Comment>? Recives { get; set;}
        public Comment? commentParent { get; set; }
        public string? commentParentId { get; set; }
        public DateTime DateTime { get; set; }
        public Post Post { get; set; }
        public string PostId { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public string? imagePath { get; set; }

    }
}
