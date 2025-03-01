using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Data_Transfer_Object
{
    public class NotificationDto
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime DateTime { get; set; }
        public UserPostDto user {  get; set; }
        public string? PostId { get; set; }

    }
}
