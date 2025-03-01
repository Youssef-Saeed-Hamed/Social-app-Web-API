using Core_Layer.Entities;
using Core_Layer.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .ValueGeneratedNever();

            builder.HasMany(u => u.Posts)
                   .WithOne(p => p.User)
                   .HasForeignKey(P => P.UserId);


            builder.HasMany(u => u.Followers)
                .WithOne(f => f.Follower)
                .HasForeignKey(F => F.FollowerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.Followings)
                .WithOne(f => f.Following)
                .HasForeignKey(F => F.FollowingId)
                .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
