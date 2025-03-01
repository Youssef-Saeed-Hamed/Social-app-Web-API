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
    internal class UserFollowersConfigurations : IEntityTypeConfiguration<UserFollowers>
    {
        public void Configure(EntityTypeBuilder<UserFollowers> builder)
        {
            builder.HasKey(uf => new { uf.FollowerId, uf.FollowingId });
        }
    }
}
