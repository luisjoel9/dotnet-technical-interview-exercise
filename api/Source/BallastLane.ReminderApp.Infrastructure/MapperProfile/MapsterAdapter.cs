using BallastLane.ReminderApp.Application.Interfaces;
using Mapster;

namespace BallastLane.ReminderApp.Infrastructure.MapperProfile
{
    public class MapsterAdapter : IMapper
    {
        public TDestination Map<TDestination>(object source)
        {
            if (source == null)
            {
                return default!;
            }

            return source.Adapt<TDestination>();
        }
    }
}
