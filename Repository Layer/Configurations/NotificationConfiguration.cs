using Core_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasOne(n => n.ReciverNotification)
                .WithMany(n => n.MyNotifications)
                .HasForeignKey(n => n.ReciverNotificationId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(n => n.SenderNotification)
                .WithMany(n => n.SendingNotifications)
                .HasForeignKey(n => n.SenderNotificationId)
                .OnDelete(DeleteBehavior.Restrict);





        }
    }
}
