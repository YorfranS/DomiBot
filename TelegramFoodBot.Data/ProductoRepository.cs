using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Data
{
    public class ProductoRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();        public List<Producto> ObtenerTodosLosProductos()
        {
            var productos = new List<Producto>();
            using var con = _db.GetConnection();
            con.Open();

            string sql = @"SELECT p.Id, p.Nombre, p.Descripcion, p.Precio, p.CategoriaId, p.Estado, 
                          p.EnPromocion, p.FechaCreacion, p.FechaActualizacion, c.Nombre as CategoriaNombre
                          FROM Productos p
                          INNER JOIN Categorias c ON p.CategoriaId = c.Id
                          ORDER BY c.Nombre, p.Nombre";
            using var cmd = _db.CreateCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                productos.Add(new Producto
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Precio = reader.GetDecimal(3),
                    CategoriaId = reader.GetInt32(4),
                    Estado = reader.GetBoolean(5),
                    EnPromocion = reader.GetBoolean(6),
                    FechaCreacion = reader.GetDateTime(7),
                    FechaActualizacion = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                    CategoriaNombre = reader.GetString(9)
                });
            }

            return productos;
        }        public List<Producto> ObtenerProductosActivos()
        {
            var productos = new List<Producto>();
            using var con = _db.GetConnection();
            con.Open();

            string sql = @"SELECT p.Id, p.Nombre, p.Descripcion, p.Precio, p.CategoriaId, p.Estado, 
                          p.EnPromocion, p.FechaCreacion, p.FechaActualizacion, c.Nombre as CategoriaNombre
                          FROM Productos p
                          INNER JOIN Categorias c ON p.CategoriaId = c.Id
                          WHERE p.Estado = 1
                          ORDER BY c.Nombre, p.Nombre";
            using var cmd = _db.CreateCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                productos.Add(new Producto
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Precio = reader.GetDecimal(3),
                    CategoriaId = reader.GetInt32(4),
                    Estado = reader.GetBoolean(5),
                    EnPromocion = reader.GetBoolean(6),
                    FechaCreacion = reader.GetDateTime(7),
                    FechaActualizacion = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                    CategoriaNombre = reader.GetString(9)
                });
            }

            return productos;
        }

        // NUEVO MÉTODO: Obtener productos en promoción activa
        public List<Producto> ObtenerProductosEnPromocion()
        {
            var productos = new List<Producto>();
            using var con = _db.GetConnection();
            con.Open();

            string sql = @"SELECT p.Id, p.Nombre, p.Descripcion, p.Precio, p.CategoriaId, p.Estado, 
                          p.EnPromocion, p.FechaCreacion, p.FechaActualizacion, c.Nombre as CategoriaNombre
                          FROM Productos p
                          INNER JOIN Categorias c ON p.CategoriaId = c.Id                          WHERE p.Estado = 1 AND p.EnPromocion = 1
                          ORDER BY c.Nombre, p.Nombre";
            using var cmd = _db.CreateCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                productos.Add(new Producto
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Precio = reader.GetDecimal(3),
                    CategoriaId = reader.GetInt32(4),
                    Estado = reader.GetBoolean(5),
                    EnPromocion = reader.GetBoolean(6),
                    FechaCreacion = reader.GetDateTime(7),
                    FechaActualizacion = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                    CategoriaNombre = reader.GetString(9)
                });
            }

            return productos;
        }

        public List<Producto> ObtenerProductosPorCategoria(int categoriaId)
        {
            var productos = new List<Producto>();
            using var con = _db.GetConnection();
            con.Open();

            string sql = @"SELECT p.Id, p.Nombre, p.Descripcion, p.Precio, p.CategoriaId, p.Estado, 
                          p.EnPromocion, p.FechaCreacion, p.FechaActualizacion, c.Nombre as CategoriaNombre
                          FROM Productos p
                          INNER JOIN Categorias c ON p.CategoriaId = c.Id                          WHERE p.CategoriaId = @CategoriaId
                          ORDER BY p.Nombre";
            using var cmd = _db.CreateCommand(sql, con);
            cmd.Parameters.AddWithValue("@CategoriaId", categoriaId);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                productos.Add(new Producto
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Precio = reader.GetDecimal(3),
                    CategoriaId = reader.GetInt32(4),
                    Estado = reader.GetBoolean(5),
                    EnPromocion = reader.GetBoolean(6),
                    FechaCreacion = reader.GetDateTime(7),
                    FechaActualizacion = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                    CategoriaNombre = reader.GetString(9)
                });
            }

            return productos;
        }

        public List<Producto> ObtenerProductosActivosPorCategoria(int categoriaId)
        {
            var productos = new List<Producto>();
            using var con = _db.GetConnection();
            con.Open();

            string sql = @"SELECT p.Id, p.Nombre, p.Descripcion, p.Precio, p.CategoriaId, p.Estado, 
                          p.EnPromocion, p.FechaCreacion, p.FechaActualizacion, c.Nombre as CategoriaNombre
                          FROM Productos p
                          INNER JOIN Categorias c ON p.CategoriaId = c.Id                          WHERE p.CategoriaId = @CategoriaId AND p.Estado = 1
                          ORDER BY p.Nombre";
            using var cmd = _db.CreateCommand(sql, con);
            cmd.Parameters.AddWithValue("@CategoriaId", categoriaId);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                productos.Add(new Producto
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Precio = reader.GetDecimal(3),
                    CategoriaId = reader.GetInt32(4),
                    Estado = reader.GetBoolean(5),
                    EnPromocion = reader.GetBoolean(6),
                    FechaCreacion = reader.GetDateTime(7),
                    FechaActualizacion = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                    CategoriaNombre = reader.GetString(9)
                });
            }

            return productos;
        }

        public void AgregarProducto(Producto producto)
        {
            using var con = _db.GetConnection();
            con.Open();            string sql = "INSERT INTO Productos (Nombre, Descripcion, Precio, CategoriaId, Estado, EnPromocion, FechaCreacion) VALUES (@Nombre, @Descripcion, @Precio, @CategoriaId, @Estado, @EnPromocion, @FechaCreacion)";
            using var cmd = _db.CreateCommand(sql, con);
            cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", producto.Descripcion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Precio", producto.Precio);
            cmd.Parameters.AddWithValue("@CategoriaId", producto.CategoriaId);
            cmd.Parameters.AddWithValue("@Estado", producto.Estado);
            cmd.Parameters.AddWithValue("@EnPromocion", producto.EnPromocion);
            cmd.Parameters.AddWithValue("@FechaCreacion", producto.FechaCreacion);

            cmd.ExecuteNonQuery();
        }

        public void ActualizarProducto(Producto producto)
        {
            using var con = _db.GetConnection();
            con.Open();            string sql = "UPDATE Productos SET Nombre = @Nombre, Descripcion = @Descripcion, Precio = @Precio, CategoriaId = @CategoriaId, Estado = @Estado, EnPromocion = @EnPromocion, FechaActualizacion = @FechaActualizacion WHERE Id = @Id";
            using var cmd = _db.CreateCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", producto.Id);
            cmd.Parameters.AddWithValue("@Nombre", producto.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", producto.Descripcion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Precio", producto.Precio);
            cmd.Parameters.AddWithValue("@CategoriaId", producto.CategoriaId);
            cmd.Parameters.AddWithValue("@Estado", producto.Estado);
            cmd.Parameters.AddWithValue("@EnPromocion", producto.EnPromocion);
            cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);

            cmd.ExecuteNonQuery();
        }

        public void EliminarProducto(int id)
        {
            using var con = _db.GetConnection();
            con.Open();            string sql = "DELETE FROM Productos WHERE Id = @Id";
            using var cmd = _db.CreateCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        public void CambiarEstadoProducto(int id, bool estado)
        {
            using var con = _db.GetConnection();
            con.Open();            string sql = "UPDATE Productos SET Estado = @Estado, FechaActualizacion = @FechaActualizacion WHERE Id = @Id";
            using var cmd = _db.CreateCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Estado", estado);
            cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);

            cmd.ExecuteNonQuery();
        }

        // NUEVO MÉTODO: Cambiar estado de promoción de un producto
        public void CambiarEstadoPromocion(int id, bool enPromocion)
        {
            using var con = _db.GetConnection();
            con.Open();            string sql = "UPDATE Productos SET EnPromocion = @EnPromocion, FechaActualizacion = @FechaActualizacion WHERE Id = @Id";
            using var cmd = _db.CreateCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@EnPromocion", enPromocion);
            cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);

            cmd.ExecuteNonQuery();
        }

        public bool ExisteProducto(string nombre)
        {
            using var con = _db.GetConnection();
            con.Open();            string sql = "SELECT COUNT(*) FROM Productos WHERE Nombre = @Nombre";
            using var cmd = _db.CreateCommand(sql, con);
            cmd.Parameters.AddWithValue("@Nombre", nombre);

            return (int)cmd.ExecuteScalar() > 0;
        }

        public bool ExisteProducto(string nombre, int idExcluir)
        {
            using var con = _db.GetConnection();
            con.Open();            string sql = "SELECT COUNT(*) FROM Productos WHERE Nombre = @Nombre AND Id != @Id";
            using var cmd = _db.CreateCommand(sql, con);
            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.Parameters.AddWithValue("@Id", idExcluir);

            return (int)cmd.ExecuteScalar() > 0;
        }        public Producto ObtenerProductoPorId(int id)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = @"SELECT p.Id, p.Nombre, p.Descripcion, p.Precio, p.CategoriaId, p.Estado, 
                          p.EnPromocion, p.FechaCreacion, p.FechaActualizacion, c.Nombre as CategoriaNombre
                          FROM Productos p
                          INNER JOIN Categorias c ON p.CategoriaId = c.Id
                          WHERE p.Id = @Id";
            
            using var cmd = _db.CreateCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);
            
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Producto
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Precio = reader.GetDecimal(3),
                    CategoriaId = reader.GetInt32(4),
                    Estado = reader.GetBoolean(5),
                    EnPromocion = reader.GetBoolean(6),
                    FechaCreacion = reader.GetDateTime(7),
                    FechaActualizacion = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                    CategoriaNombre = reader.GetString(9)
                };
            }

            return null;
        }

        /// <summary>
        /// Método de diagnóstico para probar la conexión y rendimiento de la base de datos
        /// </summary>
        public bool ProbarConexionDB()
        {
            try
            {
                using var con = _db.GetConnection();
                con.Open();
                
                string sql = "SELECT 1";
                using var cmd = _db.CreateCommand(sql, con);
                var result = cmd.ExecuteScalar();
                
                return result != null && result.ToString() == "1";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en conexión DB: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene estadísticas básicas de la tabla Productos para diagnóstico
        /// </summary>
        public (int TotalProductos, int ProductosActivos, bool ConexionOK) ObtenerEstadisticasProductos()
        {
            try
            {
                using var con = _db.GetConnection();
                con.Open();
                
                string sql = @"SELECT 
                              COUNT(*) as Total,
                              SUM(CASE WHEN Estado = 1 THEN 1 ELSE 0 END) as Activos
                              FROM Productos";
                using var cmd = _db.CreateCommand(sql, con);
                using var reader = cmd.ExecuteReader();
                  if (reader.Read())
                {
                    return (
                        TotalProductos: reader.GetInt32(0),  // Index 0 = Total
                        ProductosActivos: reader.GetInt32(1), // Index 1 = Activos
                        ConexionOK: true
                    );
                }
                
                return (0, 0, false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo estadísticas: {ex.Message}");
                return (0, 0, false);
            }
        }
    }
}
