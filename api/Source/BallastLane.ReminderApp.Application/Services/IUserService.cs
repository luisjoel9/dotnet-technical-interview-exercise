using BallastLane.ReminderApp.Application.Dtos;

namespace BallastLane.ReminderApp.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto> GetByIdAsync(Guid id);
        Task<UserResponseDto> GetByEmailAsync(string email);
        Task<bool> ExistsAsync(string email);
        Task<UserResponseDto> AddAsync(UserRequestDto user);
        Task UpdateAsync(Guid id, UserRequestDto user);
        Task DeleteAsync(Guid id);
    }
}
