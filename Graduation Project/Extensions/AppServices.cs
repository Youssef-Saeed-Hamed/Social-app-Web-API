using Core_Layer.Inetrfaces.Repositries;
using Core_Layer.Inetrfaces.Services;
using Repository_Layer.Repositries;
using Service_Layer;

namespace Graduation_Project.Extensions
{
    public static class AppServices
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserServices, UserService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IFollowingService, FollowingService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ILikeService, LikeService>();
            services.AddScoped<IUserFollowersRepository, UserFollowersRepository>();

            return services;
        }
    }
}
