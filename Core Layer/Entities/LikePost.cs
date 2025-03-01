using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Entities
{
    public class LikePost : Like
    {
        public Post Post { get; set; }
        public string PostId { get; set; }
    }
}
