using BallastLane.ReminderApp.Application.Dtos;
using BallastLane.ReminderApp.Domain.Entities;
using Mapster;

namespace BallastLane.ReminderApp.Infrastucture.MapperProfile
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            var config = TypeAdapterConfig.GlobalSettings;

            config.NewConfig<ReminderRequestDto, Reminder>().TwoWays();
            config.NewConfig<Reminder, ReminderResponseDto>().TwoWays();
            config.NewConfig<ReminderRequestDto, ReminderResponseDto>().TwoWays();

            config.NewConfig<UserRequestDto, User>().TwoWays();
            config.NewConfig<User, UserResponseDto>().TwoWays();
            config.NewConfig<UserRequestDto, UserResponseDto>().TwoWays();

            config.Compile();
        }
    }
}
