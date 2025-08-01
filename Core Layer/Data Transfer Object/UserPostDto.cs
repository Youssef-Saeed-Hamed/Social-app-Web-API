using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Data_Transfer_Object
{
    public class UserPostDto
    {
        public string UserId { get; set; } 
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public bool IsFollowing {  get; set; }

    }
}
