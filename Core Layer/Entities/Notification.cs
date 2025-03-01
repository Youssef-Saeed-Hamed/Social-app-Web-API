using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Entities
{
    public class Notification : BaseEntity<string>
    {
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public User ReciverNotification { get; set; }
        public string ReciverNotificationId { get; set; }
        public User SenderNotification { get; set; }
        public string SenderNotificationId { get; set; }
        public string? PostId { get; set; }
        public bool IsRead { get; set; }


    }
}
