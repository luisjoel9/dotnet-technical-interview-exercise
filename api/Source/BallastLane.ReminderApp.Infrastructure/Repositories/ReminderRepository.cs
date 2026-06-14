using BallastLane.ReminderApp.Application.Interfaces;
using BallastLane.ReminderApp.Domain.Entities;
using BallastLane.ReminderApp.Domain.Enums;
using BallastLane.ReminderApp.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace BallastLane.ReminderApp.Infrastructure.Repositories
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly ISqlConnectionFactory _factory;

        public ReminderRepository(ISqlConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<Reminder>> GetAllAsync(Guid userId, bool? isCompleted, bool? isOverdue)
        {
            var reminders = new List<Reminder>();

            var query = "SELECT Id, UserId, Title, Description, TargetDateTime, Status FROM Reminder WHERE UserId = @UserId";

            if (isCompleted.HasValue)
            {
                if (isCompleted.Value)
                    query += " AND Status = @StatusCompleted";
                else
                    query += " AND Status != @StatusCompleted";
            }

            if (isOverdue.HasValue)
            {
                if (isOverdue.Value)
                    query += " AND Status = @StatusOverdue";
                else
                    query += " AND Status != @StatusOverdue";
            }

            using var connection = _factory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            if (isCompleted.HasValue)
            {
                command.Parameters.AddWithValue("@StatusCompleted", (int)StatusEnum.Completed);
            }
            if (isOverdue.HasValue)
            {
                command.Parameters.AddWithValue("@StatusOverdue", (int)StatusEnum.Overdue);
            }

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                reminders.Add(MapToReminder(reader));
            }

            return reminders;
        }

        public async Task<Reminder?> GetByIdAsync(Guid id)
        {
            const string query = "SELECT Id, UserId, Title, Description, TargetDateTime, Status FROM Reminder WHERE Id = @Id";

            using var connection = _factory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return MapToReminder(reader);

            return null;
        }

        public async Task<Guid> CreateAsync(Reminder reminder)
        {
            if (reminder.Id == Guid.Empty)
            {
                reminder.Id = Guid.NewGuid();
            }

            const string query = @"
                INSERT INTO Reminder (Id, UserId, Title, Description, TargetDateTime, Status) 
                VALUES (@Id, @UserId, @Title, @Description, @TargetDateTime, @Status);";

            using var connection = _factory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", reminder.Id);
            command.Parameters.AddWithValue("@UserId", reminder.UserId);
            command.Parameters.AddWithValue("@Title", reminder.Title);
            command.Parameters.AddWithValue("@Description", reminder.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@TargetDateTime", reminder.TargetDateTime);
            command.Parameters.AddWithValue("@Status", (int)reminder.Status);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return reminder.Id;
        }

        public async Task<Guid> UpdateAsync(Reminder reminder)
        {
            const string query = @"
                UPDATE Reminder 
                SET UserId = @UserId, Title = @Title, Description = @Description, TargetDateTime = @TargetDateTime, Status = @Status 
                WHERE Id = @Id";

            using var connection = _factory.CreateConnection();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", reminder.Id);
            command.Parameters.AddWithValue("@UserId", reminder.UserId);
            command.Parameters.AddWithValue("@Title", reminder.Title);
            command.Parameters.AddWithValue("@Description", reminder.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@TargetDateTime", reminder.TargetDateTime);
            command.Parameters.AddWithValue("@Status", (int)reminder.Status);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return reminder.Id;
        }

        public async Task<Guid> DeleteAsync(Guid id)
        {
            const string query = "DELETE FROM Reminder WHERE Id = @Id";

            using var connection = _factory.CreateConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return id;
        }

        private static Reminder MapToReminder(SqlDataReader reader)
        {
            return new Reminder
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? string.Empty : reader.GetString(reader.GetOrdinal("Description")),
                TargetDateTime = reader.GetDateTime(reader.GetOrdinal("TargetDateTime")),
                Status = (StatusEnum)reader.GetInt32(reader.GetOrdinal("Status"))
            };
        }
    }
}