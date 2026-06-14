using BallastLane.ReminderApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BallastLane.ReminderApp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IReminderService, ReminderService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
