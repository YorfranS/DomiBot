using System.Collections.Generic;
using System.Data.SqlClient;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Data
{
    public class CuentaPagoRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();

        public List<CuentaPago> ObtenerTodas()
        {
            var cuentas = new List<CuentaPago>();

            using var con = _db.GetConnection();
            con.Open();
            string query = "SELECT * FROM CuentasPago";

            using var cmd = new SqlCommand(query, con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cuentas.Add(new CuentaPago
                {
                    Id = (int)reader["Id"],
                    TipoCuenta = reader["TipoCuenta"].ToString(),
                    Banco = reader["Banco"].ToString(),
                    Numero = reader["Numero"].ToString(),
                    Titular = reader["Titular"].ToString(),
                    Instrucciones = reader["Instrucciones"].ToString(),
                    Estado = reader["Estado"].ToString()
                });
            }

            return cuentas;
        }

        public List<CuentaPago> ObtenerActivas()
        {
            var cuentas = new List<CuentaPago>();

            using var con = _db.GetConnection();
            con.Open();
            string query = "SELECT * FROM CuentasPago WHERE Estado = 'Activa'";

            using var cmd = new SqlCommand(query, con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cuentas.Add(new CuentaPago
                {
                    Id = (int)reader["Id"],
                    TipoCuenta = reader["TipoCuenta"].ToString(),
                    Banco = reader["Banco"].ToString(),
                    Numero = reader["Numero"].ToString(),
                    Titular = reader["Titular"].ToString(),
                    Instrucciones = reader["Instrucciones"].ToString(),
                    Estado = reader["Estado"].ToString()
                });
            }

            return cuentas;
        }

        public void Agregar(CuentaPago cuenta)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = @"INSERT INTO CuentasPago 
                        (TipoCuenta, Banco, Numero, Titular, Instrucciones, Estado) 
                        VALUES (@Tipo, @Banco, @Numero, @Titular, @Instrucciones, @Estado)";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Tipo", cuenta.TipoCuenta);
            cmd.Parameters.AddWithValue("@Banco", cuenta.Banco);
            cmd.Parameters.AddWithValue("@Numero", cuenta.Numero);
            cmd.Parameters.AddWithValue("@Titular", cuenta.Titular);
            cmd.Parameters.AddWithValue("@Instrucciones", cuenta.Instrucciones);
            cmd.Parameters.AddWithValue("@Estado", cuenta.Estado);
            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int id)
        {
            using var con = _db.GetConnection();
            con.Open();
            string sql = "DELETE FROM CuentasPago WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public void CambiarEstado(int id, string nuevoEstado)
        {
            using var con = _db.GetConnection();
            con.Open();
            string sql = "UPDATE CuentasPago SET Estado = @Estado WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public CuentaPago ObtenerPorId(int id)
        {
            using var con = _db.GetConnection();
            con.Open();
            string query = "SELECT * FROM CuentasPago WHERE Id = @Id";

            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            
            if (reader.Read())
            {
                return new CuentaPago
                {
                    Id = (int)reader["Id"],
                    TipoCuenta = reader["TipoCuenta"].ToString(),
                    Banco = reader["Banco"].ToString(),
                    Numero = reader["Numero"].ToString(),
                    Titular = reader["Titular"].ToString(),
                    Instrucciones = reader["Instrucciones"].ToString(),
                    Estado = reader["Estado"].ToString()
                };
            }

            return null;
        }
    }
}
