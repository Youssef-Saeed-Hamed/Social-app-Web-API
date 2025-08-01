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
    internal class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Post)
                .WithMany(u => u.Comments)
                .HasForeignKey(u => u.PostId)
                .OnDelete(DeleteBehavior.Cascade);
                

            builder.HasMany(c => c.Recives)
                .WithOne(c => c.commentParent)
                .HasForeignKey(c => c.commentParentId)
                .OnDelete(DeleteBehavior.Restrict); ;

        }
    }
}
