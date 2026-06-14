using Microsoft.Data.SqlClient;

namespace BallastLane.ReminderApp.Infrastructure.Data
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection CreateConnection()
            => new(_connectionString);
    }
}
