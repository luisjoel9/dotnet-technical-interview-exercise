using BallastLane.ReminderApp.Domain.Entities;

namespace BallastLane.ReminderApp.Application.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}
