namespace BallastLane.ReminderApp.Application.Interfaces
{
    public interface IMapper
    {
        TDestination Map<TDestination>(object source);
    }
}
