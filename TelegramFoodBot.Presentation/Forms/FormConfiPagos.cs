using System;
using System.Drawing;
using System.Windows.Forms;
using TelegramFoodBot.Data;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Presentation.Forms
{    public partial class FormConfiPagos : Form
    {
        private readonly CuentaPagoRepository _repo = new CuentaPagoRepository();
        private int? _cuentaSeleccionadaId = null;        public FormConfiPagos()
        {
            InitializeComponent();
            CargarCuentas();
            button1.Click += BtnAgregar_Click;
            btnEliminarCuenta.Click += BtnEliminar_Click;
            btnInhabilitarCuenta.Click += BtnInhabilitar_Click;
        }

        private void CargarCuentas()
        {
            panel5.Controls.Clear();
            var cuentas = _repo.ObtenerTodas();
            int y = 10;

            foreach (var cuenta in cuentas)
            {
                var lbl = new Label
                {
                    Text = $"{cuenta.Banco} - {cuenta.Numero} ({cuenta.Estado})",
                    Width = 400,
                    Height = 25,
                    Location = new Point(10, y),
                    BackColor = cuenta.Estado.ToLower() == "activa" ? Color.LightGreen : Color.LightCoral
                };

                lbl.Tag = cuenta.Id;                lbl.Click += (s, e) =>
                {
                    var id = (int)((Label)s).Tag;
                    var c = _repo.ObtenerTodas().Find(x => x.Id == id);
                    _cuentaSeleccionadaId = c.Id; // Guardar el ID de la cuenta seleccionada
                    comboBox3.Text = c.TipoCuenta;
                    comboBox4.Text = c.Banco;
                    textBox4.Text = c.Numero;
                    textBox2.Text = c.Titular;
                    textBox3.Text = c.Instrucciones;
                    
                    // Actualizar el texto del botón de habilitar/inhabilitar
                    ActualizarTextoBotonEstado(c.Estado);
                };

                panel5.Controls.Add(lbl);
                y += 30;
            }
        }        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar campos requeridos
                if (string.IsNullOrWhiteSpace(comboBox3.Text) ||
                    string.IsNullOrWhiteSpace(comboBox4.Text) ||
                    string.IsNullOrWhiteSpace(textBox4.Text) ||
                    string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos obligatorios:\n" +
                                  "- Tipo de cuenta\n" +
                                  "- Banco\n" +
                                  "- Número de cuenta\n" +
                                  "- Titular", 
                                  "Campos incompletos", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                    return;
                }

                var nuevaCuenta = new CuentaPago
                {
                    TipoCuenta = comboBox3.Text.Trim(),
                    Banco = comboBox4.Text.Trim(),
                    Numero = textBox4.Text.Trim(),
                    Titular = textBox2.Text.Trim(),
                    Instrucciones = textBox3.Text.Trim(),
                    Estado = "Activa"
                };

                _repo.Agregar(nuevaCuenta);
                LimpiarFormulario();
                CargarCuentas();
                
                MessageBox.Show("La cuenta ha sido agregada exitosamente.", 
                              "Cuenta agregada", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar la cuenta: {ex.Message}", 
                              "Error", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
            }
        }        private void LimpiarFormulario()
        {
            _cuentaSeleccionadaId = null;
            comboBox3.Text = "";
            comboBox4.Text = "";
            textBox4.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            
            // Restaurar texto por defecto del botón
            btnInhabilitarCuenta.Text = "      INHABILITAR CUENTA";
        }

        private void ActualizarTextoBotonEstado(string estadoActual)
        {
            if (estadoActual.ToLower() == "activa")
            {
                btnInhabilitarCuenta.Text = "      INHABILITAR CUENTA";
                btnInhabilitarCuenta.BackColor = Color.FromArgb(255, 128, 0); // Naranja
            }
            else
            {
                btnInhabilitarCuenta.Text = "      HABILITAR CUENTA";
                btnInhabilitarCuenta.BackColor = Color.FromArgb(0, 192, 0); // Verde
            }
        }private void BtnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar que hay una cuenta seleccionada
                if (_cuentaSeleccionadaId == null)
                {
                    MessageBox.Show("Por favor, seleccione una cuenta de la lista antes de eliminar.", 
                                  "Cuenta no seleccionada", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                    return;
                }

                // Obtener los datos de la cuenta para mostrar información en la confirmación
                var cuenta = _repo.ObtenerTodas().Find(x => x.Id == _cuentaSeleccionadaId);
                if (cuenta == null)
                {
                    MessageBox.Show("La cuenta seleccionada no existe o ha sido eliminada.", 
                                  "Error", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Error);
                    _cuentaSeleccionadaId = null;
                    return;
                }

                // Confirmar eliminación
                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea eliminar permanentemente la cuenta?\n\n" +
                    $"Banco: {cuenta.Banco}\n" +
                    $"Número: {cuenta.Numero}\n" +
                    $"Titular: {cuenta.Titular}\n\n" +
                    "Esta acción no se puede deshacer.",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    _repo.Eliminar(_cuentaSeleccionadaId.Value);
                    _cuentaSeleccionadaId = null;
                    LimpiarFormulario();
                    CargarCuentas();
                    
                    MessageBox.Show("La cuenta ha sido eliminada exitosamente.", 
                                  "Eliminación exitosa", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar la cuenta: {ex.Message}", 
                              "Error", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
            }
        }        private void BtnInhabilitar_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar que hay una cuenta seleccionada
                if (_cuentaSeleccionadaId == null)
                {
                    MessageBox.Show("Por favor, seleccione una cuenta de la lista antes de cambiar su estado.", 
                                  "Cuenta no seleccionada", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Warning);
                    return;
                }

                // Obtener los datos de la cuenta para mostrar información en la confirmación
                var cuenta = _repo.ObtenerTodas().Find(x => x.Id == _cuentaSeleccionadaId);
                if (cuenta == null)
                {
                    MessageBox.Show("La cuenta seleccionada no existe o ha sido eliminada.", 
                                  "Error", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Error);
                    _cuentaSeleccionadaId = null;
                    return;
                }

                // Determinar el nuevo estado y la acción
                string nuevoEstado = cuenta.Estado.ToLower() == "activa" ? "Inactiva" : "Activa";
                string accion = nuevoEstado == "Inactiva" ? "inhabilitar" : "habilitar";

                // Confirmar cambio de estado
                var confirmacion = MessageBox.Show(
                    $"¿Está seguro que desea {accion} la cuenta?\n\n" +
                    $"Banco: {cuenta.Banco}\n" +
                    $"Número: {cuenta.Numero}\n" +
                    $"Titular: {cuenta.Titular}\n" +
                    $"Estado actual: {cuenta.Estado}\n" +
                    $"Nuevo estado: {nuevoEstado}",
                    $"Confirmar {accion}",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion == DialogResult.Yes)
                {
                    _repo.CambiarEstado(_cuentaSeleccionadaId.Value, nuevoEstado);
                    CargarCuentas();
                      // Recargar los datos de la cuenta en el formulario para mostrar el nuevo estado
                    var cuentaActualizada = _repo.ObtenerTodas().Find(x => x.Id == _cuentaSeleccionadaId);
                    if (cuentaActualizada != null)
                    {
                        comboBox3.Text = cuentaActualizada.TipoCuenta;
                        comboBox4.Text = cuentaActualizada.Banco;
                        textBox4.Text = cuentaActualizada.Numero;
                        textBox2.Text = cuentaActualizada.Titular;
                        textBox3.Text = cuentaActualizada.Instrucciones;
                        
                        // Actualizar el texto del botón
                        ActualizarTextoBotonEstado(cuentaActualizada.Estado);
                    }
                    
                    MessageBox.Show($"La cuenta ha sido {accion}da exitosamente.", 
                                  "Cambio de estado exitoso", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cambiar el estado de la cuenta: {ex.Message}", 
                              "Error", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
            }        }

        // Dentro de FormConfiPagos.cs

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            // Puedes dejarlo vacío si no necesitas lógica aquí.
        }

        private void label10_Click(object sender, EventArgs e)
        {
            // Acción al hacer clic sobre el label10 (puedes dejarlo vacío si es solo decorativo).
        }

        private void labeltittleconfigpagos_Click(object sender, EventArgs e)
        {
            // Acción al hacer clic sobre el título (puedes dejarlo vacío si no tiene funcionalidad especial).
        }

    }
}
