using System.Data.SqlClient;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Data
{
    public class AdminRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();

        public bool ValidarCredenciales(string usuario, string contrasena)
        {
            using var con = _db.GetConnection();
            con.Open();            string sql = "SELECT COUNT(*) FROM Administradores WHERE Usuario = @Usuario AND Contrasena = @Contrasena";
            using var cmd = _db.CreateCommand(sql, con);
            cmd.Parameters.AddWithValue("@Usuario", usuario);
            cmd.Parameters.AddWithValue("@Contrasena", contrasena);

            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }
    }
}
