using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Entities
{
    public class LikeComment : Like
    {
        public Comment Comment { get; set; }
        public string CommentId { get; set; }
    }
}
