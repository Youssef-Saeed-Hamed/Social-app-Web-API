using Core_Layer.Entities.Identity;
using Graduation_Project.Erorrs;
using Graduation_Project.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository_Layer.Context;

namespace Graduation_Project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<IdentityDataContext>(o =>
               o.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

            builder.Services.AddDbContext<DataContext>(o =>
                o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.Configure<ApiBehaviorOptions>(o =>
            {
                o.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Where(m => m.Value.Errors.Any())
                        .SelectMany(m => m.Value.Errors).Select(m => m.ErrorMessage).ToList();

                    return new BadRequestObjectResult(new ApiValidationErrorResponse() { Errors = errors });
                };
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddLogging();

            builder.Services.AddSwaggerService();
            builder.Services.AddAppServices(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);
            
            var app = builder.Build();
            await DbIntializer.IntializeDb(app);
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.UseMiddleware<CustomExceptionHandler>(); 
            app.Run();
        }
    }
}
