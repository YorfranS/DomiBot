using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Data
{
    public class ReservaRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();        public List<Reserva> ObtenerReservasActivas()
        {
            var lista = new List<Reserva>();
            using var con = _db.GetConnection();
            con.Open();

            string sql = "SELECT * FROM Reservas WHERE Estado <> 'Cancelada' ORDER BY Fecha ASC";
            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Reserva
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Cliente = reader["Cliente"].ToString(),
                    Telefono = reader["Telefono"].ToString(),
                    CantidadPersonas = Convert.ToInt32(reader["CantidadPersonas"]),
                    Fecha = Convert.ToDateTime(reader["Fecha"]),
                    Hora = reader["Hora"].ToString(),
                    Estado = reader["Estado"].ToString(),
                    Observaciones = reader["Observaciones"].ToString(),
                    ClienteId = reader["ClienteId"] != DBNull.Value ? Convert.ToInt64(reader["ClienteId"]) : 0,
                    MontoSeguro = reader["MontoSeguro"] != DBNull.Value ? Convert.ToDecimal(reader["MontoSeguro"]) : 0,
                    SeguroPagado = reader["SeguroPagado"] != DBNull.Value && Convert.ToBoolean(reader["SeguroPagado"]),
                    FechaPagoSeguro = reader["FechaPagoSeguro"] != DBNull.Value ? Convert.ToDateTime(reader["FechaPagoSeguro"]) : (DateTime?)null,
                    MetodoPagoSeguro = reader["MetodoPagoSeguro"]?.ToString(),
                    ClienteAsistio = reader["ClienteAsistio"] != DBNull.Value && Convert.ToBoolean(reader["ClienteAsistio"]),
                    FechaAsistencia = reader["FechaAsistencia"] != DBNull.Value ? Convert.ToDateTime(reader["FechaAsistencia"]) : (DateTime?)null,
                    SeguroReembolsado = reader["SeguroReembolsado"] != DBNull.Value && Convert.ToBoolean(reader["SeguroReembolsado"]),
                    FechaCreacion = reader["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaCreacion"]) : DateTime.Now
                });
            }

            return lista;
        }        public void AgregarReserva(Reserva reserva)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = @"INSERT INTO Reservas (Cliente, Telefono, CantidadPersonas, Fecha, Hora, Estado, Observaciones, 
                           ClienteId, MontoSeguro, SeguroPagado, FechaPagoSeguro, MetodoPagoSeguro, ClienteAsistio, 
                           FechaAsistencia, SeguroReembolsado, FechaCreacion)
                           VALUES (@Cliente, @Telefono, @Cantidad, @Fecha, @Hora, @Estado, @Observaciones, 
                           @ClienteId, @MontoSeguro, @SeguroPagado, @FechaPagoSeguro, @MetodoPagoSeguro, @ClienteAsistio,
                           @FechaAsistencia, @SeguroReembolsado, @FechaCreacion)";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Cliente", reserva.Cliente);
            cmd.Parameters.AddWithValue("@Telefono", reserva.Telefono);
            cmd.Parameters.AddWithValue("@Cantidad", reserva.CantidadPersonas);
            cmd.Parameters.AddWithValue("@Fecha", reserva.Fecha);
            cmd.Parameters.AddWithValue("@Hora", reserva.Hora);
            cmd.Parameters.AddWithValue("@Estado", reserva.Estado);
            cmd.Parameters.AddWithValue("@Observaciones", reserva.Observaciones);
            cmd.Parameters.AddWithValue("@ClienteId", reserva.ClienteId);
            cmd.Parameters.AddWithValue("@MontoSeguro", reserva.MontoSeguro);
            cmd.Parameters.AddWithValue("@SeguroPagado", reserva.SeguroPagado);
            cmd.Parameters.AddWithValue("@FechaPagoSeguro", (object)reserva.FechaPagoSeguro ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MetodoPagoSeguro", (object)reserva.MetodoPagoSeguro ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ClienteAsistio", reserva.ClienteAsistio);
            cmd.Parameters.AddWithValue("@FechaAsistencia", (object)reserva.FechaAsistencia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SeguroReembolsado", reserva.SeguroReembolsado);
            cmd.Parameters.AddWithValue("@FechaCreacion", reserva.FechaCreacion);
            cmd.ExecuteNonQuery();
        }

        public void ActualizarEstado(int id, string nuevoEstado)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = "UPDATE Reservas SET Estado = @Estado WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public void ActualizarPagoSeguro(int id, decimal montoSeguro, string metodoPago)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = @"UPDATE Reservas SET 
                          SeguroPagado = 1, 
                          FechaPagoSeguro = @FechaPago, 
                          MetodoPagoSeguro = @MetodoPago,
                          MontoSeguro = @MontoSeguro,
                          Estado = 'Confirmada'
                          WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@FechaPago", DateTime.Now);
            cmd.Parameters.AddWithValue("@MetodoPago", metodoPago);
            cmd.Parameters.AddWithValue("@MontoSeguro", montoSeguro);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public void RegistrarAsistencia(int id, bool asistio)
        {
            using var con = _db.GetConnection();
            con.Open();

            string estado = asistio ? "Completada" : "NoShow";
            string sql = @"UPDATE Reservas SET 
                          ClienteAsistio = @Asistio, 
                          FechaAsistencia = @FechaAsistencia,
                          Estado = @Estado
                          WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Asistio", asistio);
            cmd.Parameters.AddWithValue("@FechaAsistencia", DateTime.Now);
            cmd.Parameters.AddWithValue("@Estado", estado);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public void ProcesarReembolsoSeguro(int id)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = "UPDATE Reservas SET SeguroReembolsado = 1 WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public Reserva ObtenerReservaPorId(int id)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = "SELECT * FROM Reservas WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Reserva
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Cliente = reader["Cliente"].ToString(),
                    Telefono = reader["Telefono"].ToString(),
                    CantidadPersonas = Convert.ToInt32(reader["CantidadPersonas"]),
                    Fecha = Convert.ToDateTime(reader["Fecha"]),
                    Hora = reader["Hora"].ToString(),
                    Estado = reader["Estado"].ToString(),
                    Observaciones = reader["Observaciones"].ToString(),
                    ClienteId = reader["ClienteId"] != DBNull.Value ? Convert.ToInt64(reader["ClienteId"]) : 0,
                    MontoSeguro = reader["MontoSeguro"] != DBNull.Value ? Convert.ToDecimal(reader["MontoSeguro"]) : 0,
                    SeguroPagado = reader["SeguroPagado"] != DBNull.Value && Convert.ToBoolean(reader["SeguroPagado"]),
                    FechaPagoSeguro = reader["FechaPagoSeguro"] != DBNull.Value ? Convert.ToDateTime(reader["FechaPagoSeguro"]) : (DateTime?)null,
                    MetodoPagoSeguro = reader["MetodoPagoSeguro"]?.ToString(),
                    ClienteAsistio = reader["ClienteAsistio"] != DBNull.Value && Convert.ToBoolean(reader["ClienteAsistio"]),
                    FechaAsistencia = reader["FechaAsistencia"] != DBNull.Value ? Convert.ToDateTime(reader["FechaAsistencia"]) : (DateTime?)null,
                    SeguroReembolsado = reader["SeguroReembolsado"] != DBNull.Value && Convert.ToBoolean(reader["SeguroReembolsado"]),
                    FechaCreacion = reader["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaCreacion"]) : DateTime.Now
                };
            }
            return null;
        }

        public List<Reserva> ObtenerReservasPendientesAprobacion()
        {
            var lista = new List<Reserva>();
            using var con = _db.GetConnection();
            con.Open();

            string sql = "SELECT * FROM Reservas WHERE Estado = 'Solicitud' ORDER BY FechaCreacion ASC";
            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Reserva
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Cliente = reader["Cliente"].ToString(),
                    Telefono = reader["Telefono"].ToString(),
                    CantidadPersonas = Convert.ToInt32(reader["CantidadPersonas"]),
                    Fecha = Convert.ToDateTime(reader["Fecha"]),
                    Hora = reader["Hora"].ToString(),
                    Estado = reader["Estado"].ToString(),
                    Observaciones = reader["Observaciones"].ToString(),
                    ClienteId = reader["ClienteId"] != DBNull.Value ? Convert.ToInt64(reader["ClienteId"]) : 0,
                    MontoSeguro = reader["MontoSeguro"] != DBNull.Value ? Convert.ToDecimal(reader["MontoSeguro"]) : 0,
                    FechaCreacion = reader["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaCreacion"]) : DateTime.Now
                });
            }

            return lista;
        }

        public List<Reserva> ObtenerReservasEsperandoPago()
        {
            var lista = new List<Reserva>();
            using var con = _db.GetConnection();
            con.Open();

            string sql = "SELECT * FROM Reservas WHERE Estado = 'Aceptada' AND SeguroPagado = 0 ORDER BY FechaCreacion ASC";
            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Reserva
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Cliente = reader["Cliente"].ToString(),
                    Telefono = reader["Telefono"].ToString(),
                    CantidadPersonas = Convert.ToInt32(reader["CantidadPersonas"]),
                    Fecha = Convert.ToDateTime(reader["Fecha"]),
                    Hora = reader["Hora"].ToString(),
                    Estado = reader["Estado"].ToString(),
                    Observaciones = reader["Observaciones"].ToString(),
                    ClienteId = reader["ClienteId"] != DBNull.Value ? Convert.ToInt64(reader["ClienteId"]) : 0,
                    MontoSeguro = reader["MontoSeguro"] != DBNull.Value ? Convert.ToDecimal(reader["MontoSeguro"]) : 0,
                    FechaCreacion = reader["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaCreacion"]) : DateTime.Now
                });
            }

            return lista;
        }
    }
}
