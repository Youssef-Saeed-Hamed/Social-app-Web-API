using Core_Layer.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Context
{
    public class IdentityContextSeed
    {
        public static async Task SeedIdentityAsync(UserManager<AppUser> _userManager, RoleManager<IdentityRole> _roleManager)
        {

            if (!_roleManager.Roles.Any()) 
            {
                string[] roleNames = { "user", "punished_user", "admin" };

                foreach (var roleName in roleNames)
                {
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }
        

            
            if (!_userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    UserName = "YoussefSaeed",
                    Email = "Youssef41@gmail.com"

                };


                await _userManager.CreateAsync(user, "P@ssw0rd12345");

            }

        }
    }
}
