using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Data
{
    public class PedidoRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();

        public void AgregarPedido(Pedido pedido, List<DetallePedido> detalles)
        {
            using var con = _db.GetConnection();
            con.Open();

            using var transaction = con.BeginTransaction();

            try
            {
                // Buscar teléfono del cliente directamente desde la tabla Clientes
                string telefonoCliente = null;
                string sqlTelefono = "SELECT Telefono FROM Clientes WHERE Id = @ClienteId";
                using (var cmdTel = new SqlCommand(sqlTelefono, con, transaction))
                {
                    cmdTel.Parameters.AddWithValue("@ClienteId", pedido.ClienteId);
                    var result = cmdTel.ExecuteScalar();
                    telefonoCliente = result != null ? result.ToString() : null;
                }                string sqlPedido = @"INSERT INTO Pedidos 
                (Id, ClienteId, TelefonoEntrega, DireccionEntrega, Productos, Observaciones, MetodoPago, Estado, FechaHora, PagoPendiente, PagoConfirmado, FechaConfirmacionPago, MetodoConfirmacionPago) 
                VALUES (@Id, @ClienteId, @Telefono, @Direccion, @Productos, @Obs, @Pago, @Estado, @FechaHora, @PagoPendiente, @PagoConfirmado, @FechaConfirmacionPago, @MetodoConfirmacionPago)";

                using var cmdPedido = new SqlCommand(sqlPedido, con, transaction);                cmdPedido.Parameters.AddWithValue("@Id", pedido.Id);
                cmdPedido.Parameters.AddWithValue("@ClienteId", pedido.ClienteId);
                cmdPedido.Parameters.AddWithValue("@Telefono", telefonoCliente ?? (object)DBNull.Value);
                cmdPedido.Parameters.AddWithValue("@Direccion", pedido.DireccionEntrega ?? (object)DBNull.Value);
                cmdPedido.Parameters.AddWithValue("@Productos", pedido.Productos ?? (object)DBNull.Value);
                cmdPedido.Parameters.AddWithValue("@Obs", pedido.Observaciones ?? (object)DBNull.Value);
                cmdPedido.Parameters.AddWithValue("@Pago", pedido.MetodoPago);
                cmdPedido.Parameters.AddWithValue("@Estado", pedido.Estado);
                cmdPedido.Parameters.AddWithValue("@FechaHora", pedido.FechaHora);
                cmdPedido.Parameters.AddWithValue("@PagoPendiente", pedido.PagoPendiente);
                cmdPedido.Parameters.AddWithValue("@PagoConfirmado", pedido.PagoConfirmado);
                cmdPedido.Parameters.AddWithValue("@FechaConfirmacionPago", pedido.FechaConfirmacionPago ?? (object)DBNull.Value);
                cmdPedido.Parameters.AddWithValue("@MetodoConfirmacionPago", pedido.MetodoConfirmacionPago ?? (object)DBNull.Value);
                cmdPedido.ExecuteNonQuery();

                string sqlDetalle = @"INSERT INTO DetallePedido (PedidoId, Producto, Cantidad) 
                                      VALUES (@PedidoId, @Producto, @Cantidad)";

                foreach (var detalle in detalles)
                {
                    using var cmdDetalle = new SqlCommand(sqlDetalle, con, transaction);
                    cmdDetalle.Parameters.AddWithValue("@PedidoId", detalle.PedidoId);
                    cmdDetalle.Parameters.AddWithValue("@Producto", detalle.Producto);
                    cmdDetalle.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                    cmdDetalle.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<Pedido> ObtenerPedidosActivos()
        {
            var pedidos = new List<Pedido>();

            using var con = _db.GetConnection();
            con.Open();

            string sql = "SELECT * FROM Pedidos WHERE Estado IN ('Pendiente', 'PendientePago') ORDER BY FechaHora DESC";
            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();            while (reader.Read())
            {
                pedidos.Add(new Pedido
                {
                    Id = reader["Id"].ToString(),
                    ClienteId = Convert.ToInt64(reader["ClienteId"]),
                    TelefonoEntrega = reader["TelefonoEntrega"]?.ToString(),
                    DireccionEntrega = reader["DireccionEntrega"]?.ToString(),
                    Productos = reader["Productos"]?.ToString(),
                    Observaciones = reader["Observaciones"]?.ToString(),
                    MetodoPago = reader["MetodoPago"]?.ToString(),
                    Estado = reader["Estado"]?.ToString(),
                    FechaHora = Convert.ToDateTime(reader["FechaHora"]),
                    PagoPendiente = reader["PagoPendiente"] != DBNull.Value ? Convert.ToBoolean(reader["PagoPendiente"]) : false,
                    PagoConfirmado = reader["PagoConfirmado"] != DBNull.Value ? Convert.ToBoolean(reader["PagoConfirmado"]) : false,
                    FechaConfirmacionPago = reader["FechaConfirmacionPago"] != DBNull.Value ? Convert.ToDateTime(reader["FechaConfirmacionPago"]) : null,
                    MetodoConfirmacionPago = reader["MetodoConfirmacionPago"]?.ToString()
                });
            }

            return pedidos;
        }

        public List<Pedido> ObtenerPedidosEnProceso()
        {
            var lista = new List<Pedido>();
            using var con = _db.GetConnection();
            con.Open();            string sql = @"
        SELECT * FROM Pedidos
        WHERE Estado NOT IN ('Pendiente', 'PendientePago', 'terminado','rechazado','cancelado')
        ORDER BY FechaHora DESC";

            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Pedido
                {
                    Id = reader["Id"].ToString(),
                    ClienteId = Convert.ToInt64(reader["ClienteId"]),
                    TelefonoEntrega = reader["TelefonoEntrega"]?.ToString(),
                    DireccionEntrega = reader["DireccionEntrega"]?.ToString(),
                    Productos = reader["Productos"]?.ToString(),
                    Observaciones = reader["Observaciones"]?.ToString(),
                    MetodoPago = reader["MetodoPago"]?.ToString(),
                    Estado = reader["Estado"].ToString(),
                    FechaHora = Convert.ToDateTime(reader["FechaHora"])
                });
            }

            return lista;
        }


        public void ActualizarEstadoPedido(string pedidoId, string nuevoEstado)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = "UPDATE Pedidos SET Estado = @Estado WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
            cmd.Parameters.AddWithValue("@Id", pedidoId);
            cmd.ExecuteNonQuery();
        }

        public List<Pedido> ObtenerHistorialPedidos(DateTime? fechaInicio = null, DateTime? fechaFin = null, string estado = null)
        {
            var pedidos = new List<Pedido>();

            using var con = _db.GetConnection();
            con.Open();

            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append("SELECT * FROM Pedidos WHERE ");

            // Si no hay filtro de estado, usamos los estados de pedidos históricos por defecto
            if (string.IsNullOrEmpty(estado))
            {
                queryBuilder.Append("(Estado = 'Terminado' OR Estado = 'Rechazado' OR Estado = 'Cancelado') ");
            }
            else
            {
                queryBuilder.Append("Estado = @Estado ");
            }

            // Añadir filtros de fecha si aplica
            if (fechaInicio.HasValue)
            {
                queryBuilder.Append("AND FechaHora >= @FechaInicio ");
            }

            if (fechaFin.HasValue)
            {
                queryBuilder.Append("AND FechaHora <= @FechaFin ");
            }

            queryBuilder.Append("ORDER BY FechaHora DESC");

            using var cmd = new SqlCommand(queryBuilder.ToString(), con);
            
            // Añadir parámetros según los filtros aplicados
            if (!string.IsNullOrEmpty(estado))
            {
                cmd.Parameters.AddWithValue("@Estado", estado);
            }
            
            if (fechaInicio.HasValue)
            {
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Value.Date);
            }
            
            if (fechaFin.HasValue)
            {
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Value.Date.AddDays(1).AddSeconds(-1)); // Hasta el final del día
            }
            
            using var reader = cmd.ExecuteReader();while (reader.Read())
            {
                pedidos.Add(new Pedido
                {
                    Id = reader["Id"].ToString(),
                    ClienteId = Convert.ToInt64(reader["ClienteId"]),
                    TelefonoEntrega = reader["TelefonoEntrega"].ToString(),
                    DireccionEntrega = reader["DireccionEntrega"].ToString(),
                    Productos = reader["Productos"].ToString(),
                    Observaciones = reader["Observaciones"].ToString(),
                    MetodoPago = reader["MetodoPago"].ToString(),
                    Estado = reader["Estado"].ToString(),
                    FechaHora = Convert.ToDateTime(reader["FechaHora"]),
                    PagoPendiente = reader["PagoPendiente"] != DBNull.Value ? Convert.ToBoolean(reader["PagoPendiente"]) : false,
                    PagoConfirmado = reader["PagoConfirmado"] != DBNull.Value ? Convert.ToBoolean(reader["PagoConfirmado"]) : false,
                    FechaConfirmacionPago = reader["FechaConfirmacionPago"] != DBNull.Value ? Convert.ToDateTime(reader["FechaConfirmacionPago"]) : null,
                    MetodoConfirmacionPago = reader["MetodoConfirmacionPago"]?.ToString()
                });
            }

            return pedidos;
        }
        public bool ExistePedidoConId(string id)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = "SELECT COUNT(1) FROM Pedidos WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        // Métodos para manejo de confirmación de pago por transferencia
        public void ConfirmarPagoPedido(string pedidoId, string metodoConfirmacion)
        {
            using var con = _db.GetConnection();
            con.Open();
              string sql = @"UPDATE Pedidos SET 
                          PagoConfirmado = 1,
                          PagoPendiente = 0,
                          FechaConfirmacionPago = @FechaConfirmacion,
                          MetodoConfirmacionPago = @MetodoConfirmacion,
                          Estado = 'Pendiente'
                          WHERE Id = @PedidoId AND Estado = 'PendientePago'";
            
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@FechaConfirmacion", DateTime.Now);
            cmd.Parameters.AddWithValue("@MetodoConfirmacion", metodoConfirmacion);
            cmd.Parameters.AddWithValue("@PedidoId", pedidoId);
            cmd.ExecuteNonQuery();
        }

        public void RechazarPagoPedido(string pedidoId)
        {
            using var con = _db.GetConnection();
            con.Open();
            
            string sql = @"UPDATE Pedidos SET Estado = 'Cancelado' WHERE Id = @PedidoId";
            
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@PedidoId", pedidoId);
            cmd.ExecuteNonQuery();
        }

        public List<Pedido> ObtenerPedidosPendientesPago()
        {
            var pedidos = new List<Pedido>();

            using var con = _db.GetConnection();
            con.Open();

            string sql = "SELECT * FROM Pedidos WHERE PagoPendiente = 1 AND PagoConfirmado = 0 AND Estado = 'PendientePago'";
            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                pedidos.Add(new Pedido
                {
                    Id = reader["Id"].ToString(),
                    ClienteId = Convert.ToInt64(reader["ClienteId"]),
                    TelefonoEntrega = reader["TelefonoEntrega"]?.ToString(),
                    DireccionEntrega = reader["DireccionEntrega"]?.ToString(),
                    Productos = reader["Productos"]?.ToString(),
                    Observaciones = reader["Observaciones"]?.ToString(),
                    MetodoPago = reader["MetodoPago"]?.ToString(),
                    Estado = reader["Estado"]?.ToString(),
                    FechaHora = Convert.ToDateTime(reader["FechaHora"]),
                    PagoPendiente = reader["PagoPendiente"] != DBNull.Value ? Convert.ToBoolean(reader["PagoPendiente"]) : false,
                    PagoConfirmado = reader["PagoConfirmado"] != DBNull.Value ? Convert.ToBoolean(reader["PagoConfirmado"]) : false,
                    FechaConfirmacionPago = reader["FechaConfirmacionPago"] != DBNull.Value ? Convert.ToDateTime(reader["FechaConfirmacionPago"]) : null,
                    MetodoConfirmacionPago = reader["MetodoConfirmacionPago"]?.ToString()
                });
            }

            return pedidos;
        }

        public Pedido ObtenerPedidoPorId(string pedidoId)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = "SELECT * FROM Pedidos WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", pedidoId);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Pedido
                {
                    Id = reader["Id"].ToString(),
                    ClienteId = Convert.ToInt64(reader["ClienteId"]),
                    TelefonoEntrega = reader["TelefonoEntrega"]?.ToString(),
                    DireccionEntrega = reader["DireccionEntrega"]?.ToString(),
                    Productos = reader["Productos"]?.ToString(),
                    Observaciones = reader["Observaciones"]?.ToString(),
                    MetodoPago = reader["MetodoPago"]?.ToString(),
                    Estado = reader["Estado"]?.ToString(),
                    FechaHora = Convert.ToDateTime(reader["FechaHora"]),
                    PagoPendiente = reader["PagoPendiente"] != DBNull.Value ? Convert.ToBoolean(reader["PagoPendiente"]) : false,
                    PagoConfirmado = reader["PagoConfirmado"] != DBNull.Value ? Convert.ToBoolean(reader["PagoConfirmado"]) : false,
                    FechaConfirmacionPago = reader["FechaConfirmacionPago"] != DBNull.Value ? Convert.ToDateTime(reader["FechaConfirmacionPago"]) : null,
                    MetodoConfirmacionPago = reader["MetodoConfirmacionPago"]?.ToString()
                };
            }

            return null;
        }
    }
}
