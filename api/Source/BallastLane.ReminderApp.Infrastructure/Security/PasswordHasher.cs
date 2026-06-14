using BallastLane.ReminderApp.Application.Interfaces;
using BCryptNet = BCrypt.Net.BCrypt;

namespace BallastLane.ReminderApp.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int WorkFactor = 12;

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            }

            return BCryptNet.HashPassword(password, WorkFactor);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            return BCryptNet.Verify(password, passwordHash);
        }
    }
}