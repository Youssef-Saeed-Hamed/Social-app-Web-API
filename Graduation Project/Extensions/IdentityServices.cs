using Core_Layer.Entities.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Repository_Layer.Context;
using System.Text;
using System.Text.Json;

namespace Graduation_Project.Extensions
{
    public static class IdentityServices
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddIdentityCore<AppUser>()
                .AddSignInManager<SignInManager<AppUser>>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityDataContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(
                opts => opts.SignIn.RequireConfirmedEmail = true               
            );
            services.Configure<DataProtectionTokenProviderOptions>(
                opts => opts.TokenLifespan = TimeSpan.FromHours(1)
            );
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Token:Issuer"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"])),
                        ValidateAudience = true,
                        ValidAudience = configuration["Token:Audiance"],
                        ValidateLifetime = true
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            // Override the default behavior.
                            context.HandleResponse();

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            var result = JsonSerializer.Serialize(new { errorMessage = "You are not authorized to access this resource." });
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden =  context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            var result = JsonSerializer.Serialize(new { message = "You do not have the necessary role to access this resource. it just for admin" });
                            return  context.Response.WriteAsync(result);
                        }
                    };
                });
            return services;   
        }
    }
}
