using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Data_Transfer_Object
{
    public class PostDto
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public UserPostDto User { get; set; }
        public bool IsYouLike { get; set; }
        public int NumberOfLikes { get; set; }
        public int NumberOfComments { get; set; }
        public DateTime DateTime { get; set; }
        
    }
}
