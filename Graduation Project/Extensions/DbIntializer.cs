using Core_Layer.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository_Layer.Context;

namespace Graduation_Project.Extensions
{
    public class DbIntializer
    {
        public static async Task IntializeDb(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var LoggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var context = services.GetRequiredService<DataContext>();
                    var IdentityContext = services.GetRequiredService<IdentityDataContext>();
                    var userManger = services.GetRequiredService<UserManager<AppUser>>();
                    var roleManger = services.GetRequiredService<RoleManager<IdentityRole>>();
                    if ((await context.Database.GetPendingMigrationsAsync()).Any())
                    {
                        await context.Database.MigrateAsync();
                    }
                    if((await IdentityContext.Database.GetPendingMigrationsAsync()).Any())
                    {
                        await IdentityContext.Database.MigrateAsync();
                    }
                   // await DbContextSeed.SeedAsync(context);
                    await IdentityContextSeed.SeedIdentityAsync(userManger, roleManger);

                }
                catch (Exception ex)
                {
                    var logger = LoggerFactory.CreateLogger<Program>();
                    logger.LogError(ex.Message);
                }
            }
        }
    }
}
