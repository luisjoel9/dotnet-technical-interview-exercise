using Microsoft.Data.SqlClient;

namespace BallastLane.ReminderApp.Infrastructure.Data
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}
