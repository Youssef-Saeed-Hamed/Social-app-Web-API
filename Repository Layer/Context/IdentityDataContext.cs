using Core_Layer.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Context
{
    public class IdentityDataContext : IdentityDbContext<AppUser>
    {

        public IdentityDataContext(DbContextOptions<IdentityDataContext> options) : base(options) { }
    }
}
