using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public bool IsBlind { get; set; }
        public int counterOfBullying { get; set; } = 5;
        public DateTime? PunishedUntil { get; set; }
    }
}
