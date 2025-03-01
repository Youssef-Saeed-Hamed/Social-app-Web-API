using Core_Layer.Data_Transfer_Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Inetrfaces.Services
{
    public interface INotificationService
    {
        public Task<IEnumerable<NotificationDto>> GetAllNotifications(string ReciverId);
        public Task InsertNotification(string ReciveUserId, string SendUserId,string? PostId , string Message);
    }
}
