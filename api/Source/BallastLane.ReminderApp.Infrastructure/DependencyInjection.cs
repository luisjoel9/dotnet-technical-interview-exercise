using BallastLane.ReminderApp.Application.Interfaces;
using BallastLane.ReminderApp.Infrastructure.Data;
using BallastLane.ReminderApp.Infrastructure.MapperProfile;
using BallastLane.ReminderApp.Infrastructure.Repositories;
using BallastLane.ReminderApp.Infrastructure.Security;
using BallastLane.ReminderApp.Infrastucture.MapperProfile;
using BallastLane.UserApp.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BallastLane.ReminderApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
            services.AddScoped<IReminderRepository, ReminderRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            MapsterConfig.RegisterMappings();
            services.AddSingleton<IMapper, MapsterAdapter>();

            return services;
        }
    }
}
