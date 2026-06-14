using BallastLane.ReminderApp.Domain.Entities;
using BallastLane.ReminderApp.Domain.Enums;
using BallastLane.ReminderApp.Infrastructure.Data;
using BallastLane.UserApp.Application.Interfaces;
using Microsoft.Data.SqlClient;

namespace BallastLane.ReminderApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ISqlConnectionFactory _factory;

        public UserRepository(ISqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = new Dictionary<Guid, User>();

            const string query = @"
                SELECT u.Id, u.Username, u.Email, u.Password, u.CreatedAt,
                       r.Id AS ReminderId, r.UserId AS ReminderUserId,
                       r.Title, r.Description, r.TargetDateTime, r.Status
                FROM [User] u
                LEFT JOIN Reminder r ON u.Id = r.UserId
                ORDER BY u.Username";

            using var connection = _factory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var userId = reader.GetGuid(reader.GetOrdinal("Id"));

                if (!users.TryGetValue(userId, out var user))
                {
                    user = MapToUser(reader);
                    users.Add(userId, user);
                }

                if (!reader.IsDBNull(reader.GetOrdinal("ReminderId")))
                {
                    user.Reminders.Add(MapToReminder(reader));
                }
            }

            return users.Values;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            User user = null;

            const string query = @"
            SELECT u.Id, u.Username, u.Email, u.Password, u.CreatedAt,
                   r.Id AS ReminderId, r.UserId AS ReminderUserId, r.Title, r.Description, r.TargetDateTime, r.Status
            FROM [User] u
            LEFT JOIN Reminder r ON u.Id = r.UserId
            WHERE u.Id = @Id";

            using var connection = _factory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (user == null)
                {
                    user = MapToUser(reader);
                }

                if (!reader.IsDBNull(reader.GetOrdinal("ReminderId")))
                {
                    user.Reminders.Add(MapToReminder(reader));
                }
            }

            return user;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            User user = null;

            const string query = @"
            SELECT u.Id, u.Username, u.Email, u.Password, u.CreatedAt,
                   r.Id AS ReminderId, r.UserId AS ReminderUserId, r.Title, r.Description, r.TargetDateTime, r.Status
            FROM [User] u
            LEFT JOIN Reminder r ON u.Id = r.UserId
            WHERE u.Email = @Email";

            using var connection = _factory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (user == null)
                {
                    user = MapToUser(reader);
                }

                if (!reader.IsDBNull(reader.GetOrdinal("ReminderId")))
                {
                    user.Reminders.Add(MapToReminder(reader));
                }
            }

            return user;
        }

        public async Task<bool> ExistsEmailAsync(string email)
        {
            const string query = "SELECT COUNT(1) FROM [User] WHERE Email = @Email";

            using var connection = _factory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            await connection.OpenAsync();
            var count = (int)await command.ExecuteScalarAsync();

            return count > 0;
        }

        public async Task<Guid> CreateAsync(User user)
        {
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
            }

            if (user.CreatedAt == default)
            {
                user.CreatedAt = DateTime.UtcNow;
            }

            const string query = @"
            INSERT INTO [User] (Id, Username, Email, Password, CreatedAt)
            VALUES (@Id, @Username, @Email, @Password, @CreatedAt);";

            using var connection = _factory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return user.Id;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            const string query = @"
            UPDATE [User]
            SET Username = @Username, Email = @Email, Password = @Password
            WHERE Id = @Id";

            using var connection = _factory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Password", user.Password);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            const string query = "DELETE FROM [User] WHERE Id = @Id";

            using var connection = _factory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        private static User MapToUser(SqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Password = reader.GetString(reader.GetOrdinal("Password")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                Reminders = new List<Reminder>()
            };
        }

        private static Reminder MapToReminder(SqlDataReader reader)
        {
            return new Reminder
            {
                Id = reader.GetGuid(reader.GetOrdinal("ReminderId")),
                UserId = reader.GetGuid(reader.GetOrdinal("ReminderUserId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? string.Empty : reader.GetString(reader.GetOrdinal("Description")),
                TargetDateTime = reader.GetDateTime(reader.GetOrdinal("TargetDateTime")),
                Status = (StatusEnum)reader.GetInt32(reader.GetOrdinal("Status"))
            };
        }
    }
}
