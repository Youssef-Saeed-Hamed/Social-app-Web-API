using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Entities
{
   public class Like : BaseEntity<string>
    {
        public User User { get; set; }
        public string UserId { get; set; }
    }
}
