using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TelegramFoodBot.Business.Services;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Presentation.Forms
{
    public partial class FormConfiMenu : Form
    {
        private CategoriaService _categoriaService;
        private ProductoService _productoService;
        private DataGridView dgvCategorias;
        private DataGridView dgvProductos;
        private Categoria _categoriaSeleccionada;
        private Producto _productoSeleccionado;
        private bool _modoEdicionCategoria = false;
        private bool _modoEdicionProducto = false;

        public FormConfiMenu()
        {
            InitializeComponent();
            _categoriaService = new CategoriaService();
            _productoService = new ProductoService();
            InicializarComponentes();
            CargarDatos();
        }        private void InicializarComponentes()
        {
            // Configurar ComboBoxes de estado
            comboBoxEstadoCategoria.SelectedIndex = 0; // ACTIVO por defecto
            comboBoxEstadoProducto.SelectedIndex = 0; // ACTIVO por defecto
            comboBoxPromocion.SelectedIndex = 1; // INACTIVO por defecto para promociones

            // Crear y configurar DataGridView para Categorías
            CrearDataGridViewCategorias();
            
            // Crear y configurar DataGridView para Productos
            CrearDataGridViewProductos();

            // Configurar eventos
            ConfigurarEventos();
        }

        private void CrearDataGridViewCategorias()
        {
            dgvCategorias = new DataGridView();
            dgvCategorias.Location = new Point(28, 165);
            dgvCategorias.Size = new Size(212, 220);
            dgvCategorias.BackgroundColor = Color.White;
            dgvCategorias.BorderStyle = BorderStyle.Fixed3D;
            dgvCategorias.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCategorias.MultiSelect = false;
            dgvCategorias.ReadOnly = true;
            dgvCategorias.AllowUserToAddRows = false;
            dgvCategorias.AllowUserToDeleteRows = false;
            dgvCategorias.RowHeadersVisible = false;
            dgvCategorias.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            // Configurar columnas
            dgvCategorias.Columns.Add("Id", "ID");
            dgvCategorias.Columns.Add("Nombre", "Nombre");
            dgvCategorias.Columns.Add("Estado", "Estado");
            
            dgvCategorias.Columns["Id"].Visible = false;
            dgvCategorias.Columns["Estado"].Width = 80;

            // Eventos
            dgvCategorias.CellClick += DgvCategorias_CellClick;

            panelCONTENEDORCATEGORY.Controls.Add(dgvCategorias);
        }        private void CrearDataGridViewProductos()
        {
            dgvProductos = new DataGridView();
            dgvProductos.Location = new Point(10, 10);
            dgvProductos.Size = new Size(535, 560);
            dgvProductos.BackgroundColor = Color.White;
            dgvProductos.BorderStyle = BorderStyle.Fixed3D;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.MultiSelect = false;
            dgvProductos.ReadOnly = true;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;
            dgvProductos.RowHeadersVisible = false;
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
              // Configurar columnas
            dgvProductos.Columns.Add("Id", "ID");
            dgvProductos.Columns.Add("Nombre", "Nombre");
            dgvProductos.Columns.Add("Precio", "Precio");
            dgvProductos.Columns.Add("Estado", "Estado");
            dgvProductos.Columns.Add("Promocion", "Promoción");
            dgvProductos.Columns.Add("CategoriaNombre", "Categoría");
            
            dgvProductos.Columns["Id"].Visible = false;
            dgvProductos.Columns["Precio"].Width = 80;
            dgvProductos.Columns["Estado"].Width = 80;
            dgvProductos.Columns["Promocion"].Width = 80;

            // Eventos
            dgvProductos.CellClick += DgvProductos_CellClick;

            panelMostrarProductos.Controls.Add(dgvProductos);
        }

        private void ConfigurarEventos()
        {
            // Eventos de botones de categorías
            btnAgregarCategoria.Click += BtnAgregarCategoria_Click;
            btnInhabilitarCategoria.Click += BtnInhabilitarCategoria_Click;
            btnEliminarCategoria.Click += BtnEliminarCategoria_Click;            // Eventos de botones de productos
            btnAgregarProducto.Click += BtnAgregarProducto_Click;
            btnInhabilitarProducto.Click += BtnInhabilitarProducto_Click;
            btnEliminarProducto.Click += BtnEliminarProducto_Click;
            btnTogglePromocion.Click += BtnTogglePromocion_Click;

            // Eventos de validación
            textBoxPrecioProducto.KeyPress += TextBoxPrecio_KeyPress;
            textBoxNombreCategoria.KeyPress += TextBox_KeyPress;
            textBoxNombreProducto.KeyPress += TextBox_KeyPress;
        }

        private void CargarDatos()
        {
            CargarCategorias();
            CargarProductos();
            CargarCategoriasEnComboBox();
        }

        private void CargarCategorias()
        {
            try
            {
                var categorias = _categoriaService.ObtenerTodasLasCategorias();
                dgvCategorias.Rows.Clear();

                foreach (var categoria in categorias)
                {
                    dgvCategorias.Rows.Add(
                        categoria.Id,
                        categoria.Nombre,
                        categoria.Estado ? "ACTIVO" : "INACTIVO"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar categorías: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        private void CargarProductos()
        {
            try
            {
                var productos = _productoService.ObtenerTodosLosProductos();
                dgvProductos.Rows.Clear();

                foreach (var producto in productos)
                {
                    dgvProductos.Rows.Add(
                        producto.Id,
                        producto.Nombre,
                        $"${producto.Precio:F2}",
                        producto.Estado ? "ACTIVO" : "INACTIVO",
                        producto.EnPromocion ? "ACTIVO" : "INACTIVO",
                        producto.CategoriaNombre
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarCategoriasEnComboBox()
        {
            try
            {
                var categorias = _categoriaService.ObtenerCategoriasActivas();
                comboBoxCategoriaProducto.DataSource = null;
                comboBoxCategoriaProducto.DataSource = categorias;
                comboBoxCategoriaProducto.DisplayMember = "Nombre";
                comboBoxCategoriaProducto.ValueMember = "Id";
                comboBoxCategoriaProducto.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar categorías en combo: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Eventos de DataGridView

        private void DgvCategorias_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvCategorias.Rows[e.RowIndex];
                _categoriaSeleccionada = new Categoria
                {
                    Id = (int)row.Cells["Id"].Value,
                    Nombre = row.Cells["Nombre"].Value.ToString(),
                    Estado = row.Cells["Estado"].Value.ToString() == "ACTIVO"
                };

                // Cargar datos en los controles
                textBoxNombreCategoria.Text = _categoriaSeleccionada.Nombre;
                comboBoxEstadoCategoria.SelectedItem = _categoriaSeleccionada.Estado ? "ACTIVO" : "INACTIVO";
                
                _modoEdicionCategoria = true;
                btnAgregarCategoria.Text = "  ACTUALIZAR";
            }
        }        private void DgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvProductos.Rows[e.RowIndex];
                _productoSeleccionado = _productoService.ObtenerProductoPorId((int)row.Cells["Id"].Value);

                if (_productoSeleccionado != null)
                {
                    // Cargar datos en los controles
                    textBoxNombreProducto.Text = _productoSeleccionado.Nombre;
                    textBoxDescripcionProducto.Text = _productoSeleccionado.Descripcion;
                    textBoxPrecioProducto.Text = _productoSeleccionado.Precio.ToString("F2");
                    comboBoxEstadoProducto.SelectedItem = _productoSeleccionado.Estado ? "ACTIVO" : "INACTIVO";
                    comboBoxPromocion.SelectedItem = _productoSeleccionado.EnPromocion ? "ACTIVO" : "INACTIVO";
                    comboBoxCategoriaProducto.SelectedValue = _productoSeleccionado.CategoriaId;
                    
                    _modoEdicionProducto = true;
                    btnAgregarProducto.Text = "  ACTUALIZAR";
                }
            }
        }

        #endregion

        #region Eventos de Botones de Categoría

        private void BtnAgregarCategoria_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBoxNombreCategoria.Text))
                {
                    MessageBox.Show("Por favor ingrese un nombre para la categoría.", "Validación", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool estado = comboBoxEstadoCategoria.SelectedItem?.ToString() == "ACTIVO";

                if (_modoEdicionCategoria && _categoriaSeleccionada != null)
                {
                    // Actualizar categoría existente
                    _categoriaService.ActualizarCategoria(_categoriaSeleccionada.Id, 
                        textBoxNombreCategoria.Text, estado);
                    MessageBox.Show("Categoría actualizada exitosamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Crear nueva categoría
                    _categoriaService.AgregarCategoria(textBoxNombreCategoria.Text, estado);
                    MessageBox.Show("Categoría agregada exitosamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LimpiarFormularioCategoria();
                CargarCategorias();
                CargarCategoriasEnComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnInhabilitarCategoria_Click(object sender, EventArgs e)
        {
            if (_categoriaSeleccionada == null)
            {
                MessageBox.Show("Por favor seleccione una categoría.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool nuevoEstado = !_categoriaSeleccionada.Estado;
                string accion = nuevoEstado ? "habilitar" : "inhabilitar";
                
                var result = MessageBox.Show($"¿Está seguro que desea {accion} la categoría '{_categoriaSeleccionada.Nombre}'?", 
                    "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _categoriaService.CambiarEstadoCategoria(_categoriaSeleccionada.Id, nuevoEstado);
                    MessageBox.Show($"Categoría {(nuevoEstado ? "habilitada" : "inhabilitada")} exitosamente.", 
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    LimpiarFormularioCategoria();
                    CargarCategorias();
                    CargarCategoriasEnComboBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminarCategoria_Click(object sender, EventArgs e)
        {
            if (_categoriaSeleccionada == null)
            {
                MessageBox.Show("Por favor seleccione una categoría.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var result = MessageBox.Show($"¿Está seguro que desea eliminar la categoría '{_categoriaSeleccionada.Nombre}'?\n\nEsta acción no se puede deshacer.", 
                    "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    _categoriaService.EliminarCategoria(_categoriaSeleccionada.Id);
                    MessageBox.Show("Categoría eliminada exitosamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    LimpiarFormularioCategoria();
                    CargarCategorias();
                    CargarCategoriasEnComboBox();
                    CargarProductos(); // Recargar productos por si había referencias
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Eventos de Botones de Producto

        private void BtnAgregarProducto_Click(object sender, EventArgs e)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(textBoxNombreProducto.Text))
                {
                    MessageBox.Show("Por favor ingrese un nombre para el producto.", "Validación", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (comboBoxCategoriaProducto.SelectedValue == null)
                {
                    MessageBox.Show("Por favor seleccione una categoría.", "Validación", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string errorPrecio = _productoService.ValidarPrecio(textBoxPrecioProducto.Text);
                if (!string.IsNullOrEmpty(errorPrecio))
                {
                    MessageBox.Show(errorPrecio, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }                decimal precio = decimal.Parse(textBoxPrecioProducto.Text);
                int categoriaId = (int)comboBoxCategoriaProducto.SelectedValue;
                bool estado = comboBoxEstadoProducto.SelectedItem?.ToString() == "ACTIVO";
                bool enPromocion = comboBoxPromocion.SelectedItem?.ToString() == "ACTIVO";

                if (_modoEdicionProducto && _productoSeleccionado != null)
                {
                    // Actualizar producto existente
                    _productoService.ActualizarProducto(_productoSeleccionado.Id, 
                        textBoxNombreProducto.Text, textBoxDescripcionProducto.Text, 
                        precio, categoriaId, estado, enPromocion);
                    MessageBox.Show("Producto actualizado exitosamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Crear nuevo producto
                    _productoService.AgregarProducto(textBoxNombreProducto.Text, 
                        textBoxDescripcionProducto.Text, precio, categoriaId, estado, enPromocion);
                    MessageBox.Show("Producto agregado exitosamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LimpiarFormularioProducto();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnInhabilitarProducto_Click(object sender, EventArgs e)
        {
            if (_productoSeleccionado == null)
            {
                MessageBox.Show("Por favor seleccione un producto.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool nuevoEstado = !_productoSeleccionado.Estado;
                string accion = nuevoEstado ? "habilitar" : "inhabilitar";
                
                var result = MessageBox.Show($"¿Está seguro que desea {accion} el producto '{_productoSeleccionado.Nombre}'?", 
                    "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _productoService.CambiarEstadoProducto(_productoSeleccionado.Id, nuevoEstado);
                    MessageBox.Show($"Producto {(nuevoEstado ? "habilitado" : "inhabilitado")} exitosamente.", 
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    LimpiarFormularioProducto();
                    CargarProductos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        private void BtnEliminarProducto_Click(object sender, EventArgs e)
        {
            if (_productoSeleccionado == null)
            {
                MessageBox.Show("Por favor seleccione un producto.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var result = MessageBox.Show($"¿Está seguro que desea eliminar el producto '{_productoSeleccionado.Nombre}'?\n\nEsta acción no se puede deshacer.", 
                    "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    _productoService.EliminarProducto(_productoSeleccionado.Id);
                    MessageBox.Show("Producto eliminado exitosamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    LimpiarFormularioProducto();
                    CargarProductos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTogglePromocion_Click(object sender, EventArgs e)
        {
            if (_productoSeleccionado == null)
            {
                MessageBox.Show("Por favor seleccione un producto.", "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool nuevoEstadoPromocion = !_productoSeleccionado.EnPromocion;
                string accion = nuevoEstadoPromocion ? "activar promoción" : "desactivar promoción";
                
                var result = MessageBox.Show($"¿Está seguro que desea {accion} para el producto '{_productoSeleccionado.Nombre}'?", 
                    "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _productoService.CambiarEstadoPromocion(_productoSeleccionado.Id, nuevoEstadoPromocion);
                    MessageBox.Show($"Promoción {(nuevoEstadoPromocion ? "activada" : "desactivada")} exitosamente.", 
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Actualizar el estado local del producto seleccionado
                    _productoSeleccionado.EnPromocion = nuevoEstadoPromocion;
                    
                    // Actualizar el comboBox de promoción
                    comboBoxPromocion.SelectedItem = nuevoEstadoPromocion ? "ACTIVO" : "INACTIVO";
                    
                    // Recargar la lista de productos para reflejar el cambio
                    CargarProductos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Métodos de Limpieza

        private void LimpiarFormularioCategoria()
        {
            textBoxNombreCategoria.Clear();
            comboBoxEstadoCategoria.SelectedIndex = 0;
            _categoriaSeleccionada = null;
            _modoEdicionCategoria = false;
            btnAgregarCategoria.Text = "  AGREGAR ";
            dgvCategorias.ClearSelection();
        }        private void LimpiarFormularioProducto()
        {
            textBoxNombreProducto.Clear();
            textBoxDescripcionProducto.Clear();
            textBoxPrecioProducto.Clear();
            comboBoxEstadoProducto.SelectedIndex = 0;
            comboBoxPromocion.SelectedIndex = 1; // INACTIVO por defecto para promociones
            comboBoxCategoriaProducto.SelectedIndex = -1;
            _productoSeleccionado = null;
            _modoEdicionProducto = false;
            btnAgregarProducto.Text = "  AGREGAR ";
            dgvProductos.ClearSelection();
        }

        #endregion

        #region Eventos de Validación

        private void TextBoxPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo números, punto decimal y backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Permitir solo un punto decimal
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir letras, números, espacios, backspace y caracteres especiales básicos
            if (char.IsControl(e.KeyChar) || char.IsLetterOrDigit(e.KeyChar) || char.IsWhiteSpace(e.KeyChar) || 
                e.KeyChar == '.' || e.KeyChar == ',' || e.KeyChar == '-' || e.KeyChar == '_')
            {
                return;
            }
            e.Handled = true;
        }

        #endregion

        private void labelTITLECHAT_Click(object sender, EventArgs e)
        {

        }
    }
}
