using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Data
{
    public class ClienteRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();

        public void AgregarCliente(Client cliente)
        {
            using var con = _db.GetConnection();
            con.Open();            string sql = "IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Id = @Id) " +
                         "INSERT INTO Clientes (Id, Nombre, Telefono, Username) VALUES (@Id, @Nombre, @Telefono, @Username)";
            using var cmd = _db.CreateCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", cliente.Id);
            cmd.Parameters.AddWithValue("@Nombre", cliente.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefono", cliente.Phone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Username", cliente.Username ?? (object)DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public List<Client> ObtenerClientes()
        {
            var lista = new List<Client>();
            using var con = _db.GetConnection();
            con.Open();
            using var cmd = _db.CreateCommand("SELECT Id, Nombre, Telefono, Username FROM Clientes", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Client
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    Phone = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Username = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }
            return lista;
        }

        public bool ExisteCliente(long id)
        {
            using var con = _db.GetConnection();
            con.Open();            string sql = "SELECT COUNT(1) FROM Clientes WHERE Id = @Id";
            using var cmd = _db.CreateCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }
    }
}
