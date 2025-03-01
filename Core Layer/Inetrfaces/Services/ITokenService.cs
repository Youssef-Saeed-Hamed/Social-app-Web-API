using Core_Layer.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Inetrfaces.Services
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(AppUser appUser);
    }
}
