using Core_Layer.Entities.Identity;
using Core_Layer.Inetrfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public TokenService(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> GenerateToken(AppUser appUser)
        {
            // make claims for email and Id and user name 
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email , appUser.Email),
                new Claim(ClaimTypes.NameIdentifier , appUser.Id),
                new Claim(ClaimTypes.Name , appUser.UserName),
                new Claim("IsBlind" , appUser.IsBlind.ToString()),
                new Claim("PunishedUntil" , appUser.PunishedUntil.ToString() ?? "")


            };

            // add roles as claims
            var roles = await _userManager.GetRolesAsync(appUser);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // make an object from symmetric security key and pass the key but in bytes
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Key"]));

            // make an object form signing credential and pass the key and type of algorithm
            var credential = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);


            // make an object from security token descriptor 
            var TokenDescriptor = new SecurityTokenDescriptor()
            {
                // subject = new object from claims identity and pass claims to it 
                Subject = new ClaimsIdentity(claims),
                // Issuer = issuer in app settings
                Issuer = _configuration["Token:Issuer"],
                // audiance = audiance in app settings
                Audience = _configuration["Token:Audiance"],

                // Epires date for this token 
                Expires = DateTime.Now.AddDays(20),

                // signing credential = credential 
                SigningCredentials = credential

            };

            // make new object from jwtsecurity token handleer
            var TokenHandler = new JwtSecurityTokenHandler();
            
            var token = TokenHandler.CreateToken(TokenDescriptor);
            return TokenHandler.WriteToken(token);


        }
    }
}
