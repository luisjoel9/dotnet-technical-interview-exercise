using BallastLane.ReminderApp.Domain.Entities;

namespace BallastLane.ReminderApp.Application.Interfaces
{
    public interface IReminderRepository
    {
        Task<IEnumerable<Reminder>> GetAllAsync(Guid userId, bool? isCompleted, bool? isOverdue);
        Task<Reminder?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(Reminder reminder);
        Task<Guid> UpdateAsync(Reminder reminder);
        Task<Guid> DeleteAsync(Guid id);
    }
}
