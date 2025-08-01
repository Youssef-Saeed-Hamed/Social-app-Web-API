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
    public class LikePostConfigurations : IEntityTypeConfiguration<LikePost>
    {
        public void Configure(EntityTypeBuilder<LikePost> builder)
        {
            builder.HasOne<User>(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne<Post>(x => x.Post)
                .WithMany(l => l.LikePosts)
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
