using System;
using System.Collections.Generic;
using System.Linq;
using TelegramFoodBot.Entities.Models;
using System.Data.SqlClient;
#pragma warning disable CS0618 // Deshabilita advertencia de obsolescencia para SqlCommand
namespace TelegramFoodBot.Data
{
    public class MessageRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();

        public void GuardarMensaje(Message mensaje)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = @"INSERT INTO Mensajes 
                        (ClientId, Content, PhotoBase64, IsFromAdmin, Timestamp, FromFirstName, FromLastName, FromUsername) 
                        VALUES (@ClientId, @Content, @PhotoBase64, @IsFromAdmin, @Timestamp, @FromFirstName, @FromLastName, @FromUsername)";

            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@ClientId", mensaje.ClientId);
            cmd.Parameters.AddWithValue("@Content", mensaje.Text ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PhotoBase64", mensaje.PhotoBase64 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IsFromAdmin", mensaje.IsFromAdmin);
            cmd.Parameters.AddWithValue("@Timestamp", mensaje.Timestamp);
            cmd.Parameters.AddWithValue("@FromFirstName", mensaje.From?.FirstName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FromLastName", mensaje.From?.LastName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FromUsername", mensaje.From?.Username ?? (object)DBNull.Value);
            
            cmd.ExecuteNonQuery();
        }

        public List<Message> ObtenerMensajesCliente(long clientId)
        {
            var mensajes = new List<Message>();
            using var con = _db.GetConnection();
            con.Open();            string sql = "SELECT * FROM Mensajes WHERE ClientId = @ClientId ORDER BY Timestamp ASC";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@ClientId", clientId);
            
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var mensaje = new Message
                {
                    ClientId = Convert.ToInt64(reader["ClientId"]),
                    Text = reader["Content"] == DBNull.Value ? null : reader["Content"].ToString(),
                    PhotoBase64 = reader["PhotoBase64"] == DBNull.Value ? null : reader["PhotoBase64"].ToString(),
                    IsFromAdmin = Convert.ToBoolean(reader["IsFromAdmin"]),
                    Timestamp = Convert.ToDateTime(reader["Timestamp"])
                };

                // Crear objeto From si hay datos
                if (reader["FromFirstName"] != DBNull.Value)
                {
                    mensaje.From = new Entities.Models.User
                    {
                        FirstName = reader["FromFirstName"].ToString(),
                        LastName = reader["FromLastName"] == DBNull.Value ? null : reader["FromLastName"].ToString(),
                        Username = reader["FromUsername"] == DBNull.Value ? null : reader["FromUsername"].ToString()
                    };
                }

                mensajes.Add(mensaje);
            }

            return mensajes;
        }

        public void MarcarMensajesComoLeidos(long clientId)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = "UPDATE Mensajes SET IsRead = 1 WHERE ClientId = @ClientId AND IsFromAdmin = 0";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@ClientId", clientId);
            cmd.ExecuteNonQuery();
        }

        public bool TieneMensajesNoLeidos(long clientId)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = "SELECT COUNT(*) FROM Mensajes WHERE ClientId = @ClientId AND IsFromAdmin = 0 AND IsRead = 0";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@ClientId", clientId);

            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        public List<long> ObtenerClientesConMensajesNoLeidos()
        {
            var clientes = new List<long>();
            using var con = _db.GetConnection();
            con.Open();            string sql = "SELECT DISTINCT ClientId FROM Mensajes WHERE IsFromAdmin = 0 AND IsRead = 0";
            using var cmd = new SqlCommand(sql, con);
            
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                clientes.Add(Convert.ToInt64(reader["ClientId"]));
            }

            return clientes;
        }
        // Devuelve una lista de (ClientId, LastMessage) para cada cliente con mensajes
        public List<(long ClientId, DateTime LastMessage)> ObtenerUltimosMensajesPorCliente()
        {
            var lista = new List<(long ClientId, DateTime LastMessage)>();
            using var con = _db.GetConnection();
            con.Open();
            string sql = @"SELECT ClientId, MAX(Timestamp) as LastMessage FROM Mensajes GROUP BY ClientId";
            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var clientId = Convert.ToInt64(reader["ClientId"]);
                var lastMessage = Convert.ToDateTime(reader["LastMessage"]);
                lista.Add((clientId, lastMessage));
            }
            return lista;
        }
    }
}
