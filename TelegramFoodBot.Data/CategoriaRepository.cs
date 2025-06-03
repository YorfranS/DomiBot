using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Data
{
    public class CategoriaRepository
    {
        private readonly DatabaseConnection _db = new DatabaseConnection();        public List<Categoria> ObtenerTodasLasCategorias()
        {
            var categorias = new List<Categoria>();
            using var con = _db.GetConnection();
            con.Open();

            string sql = "SELECT Id, Nombre, Estado, FechaCreacion, FechaActualizacion FROM Categorias ORDER BY Nombre";
            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                categorias.Add(new Categoria
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Estado = reader.GetBoolean(2),
                    FechaCreacion = reader.GetDateTime(3),
                    FechaActualizacion = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
                });
            }

            return categorias;
        }

        public List<Categoria> ObtenerCategoriasActivas()
        {
            var categorias = new List<Categoria>();
            using var con = _db.GetConnection();
            con.Open();

            string sql = "SELECT Id, Nombre, Estado, FechaCreacion, FechaActualizacion FROM Categorias WHERE Estado = 1 ORDER BY Nombre";
            using var cmd = new SqlCommand(sql, con);
            using var reader = cmd.ExecuteReader();            while (reader.Read())
            {
                categorias.Add(new Categoria
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Estado = reader.GetBoolean(2),
                    FechaCreacion = reader.GetDateTime(3),
                    FechaActualizacion = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
                });
            }

            return categorias;
        }

        public void AgregarCategoria(Categoria categoria)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = "INSERT INTO Categorias (Nombre, Estado, FechaCreacion) VALUES (@Nombre, @Estado, @FechaCreacion)";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Nombre", categoria.Nombre);
            cmd.Parameters.AddWithValue("@Estado", categoria.Estado);
            cmd.Parameters.AddWithValue("@FechaCreacion", categoria.FechaCreacion);

            cmd.ExecuteNonQuery();
        }

        public void ActualizarCategoria(Categoria categoria)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = "UPDATE Categorias SET Nombre = @Nombre, Estado = @Estado, FechaActualizacion = @FechaActualizacion WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", categoria.Id);
            cmd.Parameters.AddWithValue("@Nombre", categoria.Nombre);
            cmd.Parameters.AddWithValue("@Estado", categoria.Estado);
            cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);

            cmd.ExecuteNonQuery();
        }

        public void EliminarCategoria(int id)
        {
            using var con = _db.GetConnection();
            con.Open();

            // Verificar si hay productos asociados
            string sqlVerificar = "SELECT COUNT(*) FROM Productos WHERE CategoriaId = @Id";
            using var cmdVerificar = new SqlCommand(sqlVerificar, con);
            cmdVerificar.Parameters.AddWithValue("@Id", id);
            int productosAsociados = (int)cmdVerificar.ExecuteScalar();

            if (productosAsociados > 0)
            {
                throw new InvalidOperationException("No se puede eliminar la categoría porque tiene productos asociados.");
            }

            string sql = "DELETE FROM Categorias WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        public void CambiarEstadoCategoria(int id, bool estado)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = "UPDATE Categorias SET Estado = @Estado, FechaActualizacion = @FechaActualizacion WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Estado", estado);
            cmd.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);

            cmd.ExecuteNonQuery();
        }

        public bool ExisteCategoria(string nombre)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = "SELECT COUNT(*) FROM Categorias WHERE Nombre = @Nombre";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Nombre", nombre);

            return (int)cmd.ExecuteScalar() > 0;
        }        public bool ExisteCategoria(string nombre, int idExcluir)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = "SELECT COUNT(*) FROM Categorias WHERE Nombre = @Nombre AND Id != @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.Parameters.AddWithValue("@Id", idExcluir);

            return (int)cmd.ExecuteScalar() > 0;
        }

        public Categoria ObtenerCategoriaPorId(int id)
        {
            using var con = _db.GetConnection();
            con.Open();

            string sql = "SELECT Id, Nombre, Estado, FechaCreacion, FechaActualizacion FROM Categorias WHERE Id = @Id";
            using var cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@Id", id);
            
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Categoria
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Estado = reader.GetBoolean(2),
                    FechaCreacion = reader.GetDateTime(3),
                    FechaActualizacion = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
                };
            }

            return null;
        }
    }
}
