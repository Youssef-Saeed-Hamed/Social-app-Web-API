using Core_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Specifications
{
    public class NotificationSpecification : BaseSpecification<Notification>
    {
        public NotificationSpecification(string ReciverId) : base(n => n.ReciverNotificationId == ReciverId)
        {
            Includes.Add(n => n.SenderNotification);
        }
    }
}
