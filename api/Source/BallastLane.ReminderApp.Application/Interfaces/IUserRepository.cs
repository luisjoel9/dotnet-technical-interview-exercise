using BallastLane.ReminderApp.Domain.Entities;

namespace BallastLane.UserApp.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task<bool> ExistsEmailAsync(string email);
        Task<Guid> CreateAsync(User User);
        Task<bool> UpdateAsync(User User);
        Task<bool> DeleteAsync(Guid id);
    }
}
