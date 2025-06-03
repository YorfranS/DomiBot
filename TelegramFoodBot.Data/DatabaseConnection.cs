using System.Configuration;
using System.Data.SqlClient;

namespace TelegramFoodBot.Data
{
    public class DatabaseConnection
    {
        private readonly string connectionString;
        public const int DefaultCommandTimeout = 60; // 60 seconds

        public DatabaseConnection()
        {
            connectionString = ConfigurationManager.ConnectionStrings["ConexionSQL"].ConnectionString;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Creates a SqlCommand with a default timeout
        /// </summary>
        public SqlCommand CreateCommand(string sql, SqlConnection connection)
        {
            var cmd = new SqlCommand(sql, connection);
            cmd.CommandTimeout = DefaultCommandTimeout;
            return cmd;
        }
    }
}
