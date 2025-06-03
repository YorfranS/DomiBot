using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramFoodBot.Data;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Business.Services
{
    public class ProductoService
    {
        private readonly ProductoRepository _productoRepository;

        public ProductoService()
        {
            _productoRepository = new ProductoRepository();
        }

        public List<Producto> ObtenerTodosLosProductos()
        {
            return _productoRepository.ObtenerTodosLosProductos();
        }

        public List<Producto> ObtenerProductosActivos()
        {
            return _productoRepository.ObtenerProductosActivos();
        }        public List<Producto> ObtenerProductosEnPromocion()
        {
            return _productoRepository.ObtenerProductosEnPromocion();
        }

        public List<Producto> ObtenerProductosPorCategoria(int categoriaId)
        {
            return _productoRepository.ObtenerProductosPorCategoria(categoriaId);
        }

        public List<Producto> ObtenerProductosActivosPorCategoria(int categoriaId)
        {
            return _productoRepository.ObtenerProductosActivosPorCategoria(categoriaId);
        }

        public Producto ObtenerProductoPorId(int id)
        {
            return _productoRepository.ObtenerProductoPorId(id);
        }

        public bool AgregarProducto(string nombre, string descripcion, decimal precio, int categoriaId, bool estado = true, bool enPromocion = false)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("El nombre del producto es obligatorio.");

                if (nombre.Trim().Length < 2)
                    throw new ArgumentException("El nombre del producto debe tener al menos 2 caracteres.");

                if (nombre.Trim().Length > 100)
                    throw new ArgumentException("El nombre del producto no puede exceder los 100 caracteres.");

                if (!string.IsNullOrWhiteSpace(descripcion) && descripcion.Trim().Length > 255)
                    throw new ArgumentException("La descripción no puede exceder los 255 caracteres.");

                if (precio <= 0)
                    throw new ArgumentException("El precio debe ser mayor a cero.");

                if (precio > 999999.99m)
                    throw new ArgumentException("El precio no puede exceder $999,999.99");

                if (categoriaId <= 0)
                    throw new ArgumentException("Debe seleccionar una categoría válida.");

                // Verificar si ya existe
                if (_productoRepository.ExisteProducto(nombre.Trim()))
                    throw new InvalidOperationException("Ya existe un producto con ese nombre.");

                var producto = new Producto
                {
                    Nombre = nombre.Trim(),
                    Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim(),
                    Precio = Math.Round(precio, 2),
                    CategoriaId = categoriaId,
                    Estado = estado,
                    EnPromocion = enPromocion,
                    FechaCreacion = DateTime.Now
                };

                _productoRepository.AgregarProducto(producto);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ActualizarProducto(int id, string nombre, string descripcion, decimal precio, int categoriaId, bool estado, bool enPromocion = false)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("El nombre del producto es obligatorio.");

                if (nombre.Trim().Length < 2)
                    throw new ArgumentException("El nombre del producto debe tener al menos 2 caracteres.");

                if (nombre.Trim().Length > 100)
                    throw new ArgumentException("El nombre del producto no puede exceder los 100 caracteres.");

                if (!string.IsNullOrWhiteSpace(descripcion) && descripcion.Trim().Length > 255)
                    throw new ArgumentException("La descripción no puede exceder los 255 caracteres.");

                if (precio <= 0)
                    throw new ArgumentException("El precio debe ser mayor a cero.");

                if (precio > 999999.99m)
                    throw new ArgumentException("El precio no puede exceder $999,999.99");

                if (categoriaId <= 0)
                    throw new ArgumentException("Debe seleccionar una categoría válida.");

                // Verificar si ya existe otro con el mismo nombre
                if (_productoRepository.ExisteProducto(nombre.Trim(), id))
                    throw new InvalidOperationException("Ya existe otro producto con ese nombre.");

                var producto = new Producto
                {
                    Id = id,
                    Nombre = nombre.Trim(),
                    Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim(),
                    Precio = Math.Round(precio, 2),
                    CategoriaId = categoriaId,
                    Estado = estado,
                    EnPromocion = enPromocion,
                    FechaActualizacion = DateTime.Now
                };

                _productoRepository.ActualizarProducto(producto);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool EliminarProducto(int id)
        {
            try
            {
                _productoRepository.EliminarProducto(id);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CambiarEstadoProducto(int id, bool estado)
        {
            try
            {
                _productoRepository.CambiarEstadoProducto(id, estado);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // NUEVO MÉTODO: Cambiar estado de promoción de un producto
        public bool CambiarEstadoPromocion(int id, bool enPromocion)
        {
            try
            {
                _productoRepository.CambiarEstadoPromocion(id, enPromocion);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ExisteProducto(string nombre)
        {
            return _productoRepository.ExisteProducto(nombre);
        }

        public string ValidarPrecio(string precioTexto)
        {
            if (string.IsNullOrWhiteSpace(precioTexto))
                return "El precio es obligatorio";

            if (!decimal.TryParse(precioTexto, out decimal precio))
                return "El precio debe ser un número válido";

            if (precio <= 0)
                return "El precio debe ser mayor a cero";

            if (precio > 999999.99m)
                return "El precio no puede exceder $999,999.99";

            return null; // Sin errores
        }
    }
}
