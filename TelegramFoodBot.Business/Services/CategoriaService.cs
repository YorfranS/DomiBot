using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramFoodBot.Data;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Business.Services
{
    public class CategoriaService
    {
        private readonly CategoriaRepository _categoriaRepository;

        public CategoriaService()
        {
            _categoriaRepository = new CategoriaRepository();
        }

        public List<Categoria> ObtenerTodasLasCategorias()
        {
            return _categoriaRepository.ObtenerTodasLasCategorias();
        }

        public List<Categoria> ObtenerCategoriasActivas()
        {
            return _categoriaRepository.ObtenerCategoriasActivas();
        }

        public bool AgregarCategoria(string nombre, bool estado = true)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("El nombre de la categoría es obligatorio.");

                if (nombre.Trim().Length < 2)
                    throw new ArgumentException("El nombre de la categoría debe tener al menos 2 caracteres.");

                if (nombre.Trim().Length > 50)
                    throw new ArgumentException("El nombre de la categoría no puede exceder los 50 caracteres.");

                // Verificar si ya existe
                if (_categoriaRepository.ExisteCategoria(nombre.Trim()))
                    throw new InvalidOperationException("Ya existe una categoría con ese nombre.");

                var categoria = new Categoria
                {
                    Nombre = nombre.Trim(),
                    Estado = estado,
                    FechaCreacion = DateTime.Now
                };

                _categoriaRepository.AgregarCategoria(categoria);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ActualizarCategoria(int id, string nombre, bool estado)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new ArgumentException("El nombre de la categoría es obligatorio.");

                if (nombre.Trim().Length < 2)
                    throw new ArgumentException("El nombre de la categoría debe tener al menos 2 caracteres.");

                if (nombre.Trim().Length > 50)
                    throw new ArgumentException("El nombre de la categoría no puede exceder los 50 caracteres.");

                // Verificar si ya existe otro con el mismo nombre
                if (_categoriaRepository.ExisteCategoria(nombre.Trim(), id))
                    throw new InvalidOperationException("Ya existe otra categoría con ese nombre.");

                var categoria = new Categoria
                {
                    Id = id,
                    Nombre = nombre.Trim(),
                    Estado = estado,
                    FechaActualizacion = DateTime.Now
                };

                _categoriaRepository.ActualizarCategoria(categoria);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool EliminarCategoria(int id)
        {
            try
            {
                _categoriaRepository.EliminarCategoria(id);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CambiarEstadoCategoria(int id, bool estado)
        {
            try
            {
                _categoriaRepository.CambiarEstadoCategoria(id, estado);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }        public bool ExisteCategoria(string nombre)
        {
            return _categoriaRepository.ExisteCategoria(nombre);
        }

        public Categoria ObtenerCategoriaPorId(int id)
        {
            return _categoriaRepository.ObtenerCategoriaPorId(id);
        }
    }
}
