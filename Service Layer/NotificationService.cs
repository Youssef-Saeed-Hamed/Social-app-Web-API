using Core_Layer.Data_Transfer_Object;
using Core_Layer.Entities;
using Core_Layer.Inetrfaces.Repositries;
using Core_Layer.Inetrfaces.Services;
using Repository_Layer.Configurations;
using Repository_Layer.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<NotificationDto>> GetAllNotifications(string ReciverId)
        {
            var specs = new NotificationSpecification(ReciverId);
            var notifications = await _unitOfWork.Repositry<Notification , string>().GetAllWithSpecAsync(specs);
            var MappedNotifications = new List<NotificationDto>();
            foreach (var notification in notifications)
            {
                MappedNotifications.Add(new NotificationDto
                {
                    Id = notification.Id,
                    Message = notification.Message,
                    PostId = notification.PostId,
                    DateTime = notification.DateTime,
                    IsRead = notification.IsRead,
                    user = new UserPostDto
                    {
                        UserId = notification.SenderNotification.Id,
                        Name = $"{notification.SenderNotification.FirstName} {notification.SenderNotification.LastName}",
                        ImagePath = notification.SenderNotification.ImageUrl ?? ""
                    }
                });
            }
            for(int i = 0; i < notifications.Count(); i++)
            {
                
                var notification = notifications.ElementAt(i);
                if (notification.IsRead)
                    continue;
                notification.IsRead = true;
                _unitOfWork.Repositry<Notification , string>().Update(notification);
            }
            await _unitOfWork.CompleteAsync();

            return MappedNotifications.OrderByDescending(x => x.DateTime);

        }

        public async Task InsertNotification(string ReciveUserId, string SendUserId, string? PostId , string Message)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid().ToString(),
                DateTime = DateTime.Now,
                Message = Message,
                PostId = PostId,
                ReciverNotificationId = ReciveUserId,
                SenderNotificationId = SendUserId,
                IsRead = false
            };

            await _unitOfWork.Repositry<Notification, string>().InsertAsync(notification);
            await _unitOfWork.CompleteAsync();
        }
    }
}
