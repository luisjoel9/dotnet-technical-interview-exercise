using BallastLane.ReminderApp.Application.Dtos;

namespace BallastLane.ReminderApp.Application.Services
{
    public interface IReminderService
    {
        Task<IEnumerable<ReminderResponseDto>> GetRemindersAsync(bool? isCompleted, bool? isOverdue);
        Task<ReminderResponseDto> GetReminderByIdAsync(Guid id);
        Task<ReminderResponseDto> CreateReminderAsync(ReminderRequestDto reminder);
        Task UpdateReminderAsync(Guid id, ReminderRequestDto reminder);
        Task DeleteReminderAsync(Guid id);
    }
}
