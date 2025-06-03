using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TelegramFoodBot.Data;
using TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Presentation.Forms
{    public partial class FormHistorial : Form
    {
        private PedidoRepository _pedidoRepo = new PedidoRepository();
        private ProductoRepository _productoRepo = new ProductoRepository();
        private List<Pedido> _pedidos = new List<Pedido>();
        private Pedido _pedidoSeleccionado;

        public FormHistorial()
        {
            InitializeComponent();
            this.Load += FormHistorial_Load;
        }        private void FormHistorial_Load(object sender, EventArgs e)
        {
            // Configurar ComboBox de estados
            ConfigurarControlesDeFiltrado();
            
            // Cargar historial
            CargarHistorial();
        }
        
        private void ConfigurarControlesDeFiltrado()
        {
            // Configurar ComboBox de estados
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Todos");
            comboBox1.Items.Add("Terminado");
            comboBox1.Items.Add("Rechazado");
            comboBox1.Items.Add("Cancelado");
            comboBox1.SelectedIndex = 0;
            
            // Configurar comportamiento de filtrado
            comboBox1.SelectedIndexChanged += (s, e) => CargarHistorial();
            dateTimePicker1.ValueChanged += (s, e) => CargarHistorial();
            dateTimePicker2.ValueChanged += (s, e) => CargarHistorial();
            
            // Configurar DateTimePickers
            dateTimePicker1.ShowCheckBox = true;
            dateTimePicker2.ShowCheckBox = true;
            dateTimePicker1.Checked = false;
            dateTimePicker2.Checked = false;
            
            // Configurar búsqueda por ID de pedido
            btnBuscarPedido.Click += (s, e) => 
            {
                if (!string.IsNullOrWhiteSpace(txtBuscarPedido.Text))
                {
                    var pedido = _pedidoRepo.ObtenerPedidoPorId(txtBuscarPedido.Text);
                    if (pedido != null)
                    {
                        _pedidoSeleccionado = pedido;
                        MostrarDetallesPedido(pedido);
                    }
                    else
                    {
                        MessageBox.Show("Pedido no encontrado", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };
            
            // Configurar búsqueda por ID de cliente
            btnBuscarCliente.Click += (s, e) => 
            {
                if (!string.IsNullOrWhiteSpace(txtBuscarCliente.Text) && long.TryParse(txtBuscarCliente.Text, out long clienteId))
                {
                    panelPEDIDOSACTIVOS.Controls.Clear();
                    var pedidosCliente = _pedidoRepo.ObtenerHistorialPedidos()
                        .Where(p => p.ClienteId == clienteId)
                        .ToList();
                    
                    if (pedidosCliente.Any())
                    {
                        _pedidos = pedidosCliente;
                        CargarPedidosEnPanel();
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron pedidos para este cliente", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };
        }
        
        private void CargarPedidosEnPanel()
        {
            panelPEDIDOSACTIVOS.Controls.Clear();
            int y = 10;
            foreach (var pedido in _pedidos)
            {
                var btn = new Button
                {
                    Text = $"Pedido {pedido.Id} - {pedido.FechaHora:dd/MM HH:mm} - {pedido.Estado}",
                    Width = 580,
                    Height = 40,
                    Top = y,
                    Left = 10,
                    Tag = pedido,
                    BackColor = pedido.Estado.ToLower() switch
                    {
                        "terminado" => Color.LightGreen,
                        "rechazado" => Color.LightPink,
                        "cancelado" => Color.LightSalmon,
                        _ => Color.LightGray
                    }
                };
                btn.Click += Pedido_Click;
                panelPEDIDOSACTIVOS.Controls.Add(btn);
                y += 50;
            }
            
            label2.Text = $"Total pedidos históricos: {_pedidos.Count}";
        }private void CargarHistorial()
        {
            DateTime? fechaInicio = null;
            DateTime? fechaFin = null;
            string estadoFiltro = null;
            
            // Obtener valores de los controles de filtro si están configurados
            if (dateTimePicker1.Checked)
                fechaInicio = dateTimePicker1.Value;
                
            if (dateTimePicker2.Checked)
                fechaFin = dateTimePicker2.Value;
                
            // Obtener el estado seleccionado del combobox
            if (comboBox1.SelectedIndex > 0) // Índice 0 debe ser "Todos"
                estadoFiltro = comboBox1.SelectedItem.ToString();
                
            panelPEDIDOSACTIVOS.Controls.Clear();
            _pedidos = _pedidoRepo.ObtenerHistorialPedidos(fechaInicio, fechaFin, estadoFiltro);
            
            // Agrupar pedidos por fecha (día) y estado
            var pedidosAgrupados = _pedidos
                .GroupBy(p => new { 
                    Fecha = p.FechaHora.Date,
                    Estado = p.Estado
                })
                .OrderByDescending(g => g.Key.Fecha)
                .ThenBy(g => g.Key.Estado);
                
            int y = 10;
            
            foreach (var grupo in pedidosAgrupados)
            {
                // Crear un encabezado para cada grupo
                var fechaStr = grupo.Key.Fecha.ToString("dd/MM/yyyy");
                var lblGrupo = new Label
                {
                    Text = $"Fecha: {fechaStr} - Estado: {grupo.Key.Estado} ({grupo.Count()} pedidos)",
                    Width = 580,
                    Height = 25,
                    Top = y,
                    Left = 10,
                    Font = new Font(this.Font, FontStyle.Bold),
                    BackColor = grupo.Key.Estado.ToLower() switch
                    {
                        "terminado" => Color.LightGreen,
                        "rechazado" => Color.Pink,
                        "cancelado" => Color.LightSalmon,
                        _ => Color.LightGray
                    }
                };
                panelPEDIDOSACTIVOS.Controls.Add(lblGrupo);
                y += 30;
                
                // Agregar los pedidos de este grupo
                foreach (var pedido in grupo)
                {
                    var btn = new Button
                    {
                        Text = $"Pedido {pedido.Id} - {pedido.FechaHora:HH:mm} - Cliente: {pedido.ClienteId}",
                        Width = 560,
                        Height = 40,
                        Top = y,
                        Left = 20, // Indentado para mostrar jerarquía
                        Tag = pedido,
                        BackColor = pedido.Estado.ToLower() switch
                        {
                            "terminado" => Color.LightGreen,
                            "rechazado" => Color.LightPink,
                            "cancelado" => Color.LightSalmon,
                            _ => Color.LightGray
                        }
                    };
                    btn.Click += Pedido_Click;
                    panelPEDIDOSACTIVOS.Controls.Add(btn);
                    y += 45;
                }
                
                // Espacio entre grupos
                y += 15;
            }

            label2.Text = $"Total pedidos históricos: {_pedidos.Count}";
        }

        private void Pedido_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is Pedido pedido)
            {
                _pedidoSeleccionado = pedido;
                MostrarDetallesPedido(pedido);
            }
        }        private void MostrarDetallesPedido(Pedido pedido)
        {
            textBox9.Text = pedido.Id.ToString();
            textBox8.Text = pedido.FechaHora.ToString("g");
            textBox1.Text = pedido.ClienteId.ToString();
            textBox2.Text = pedido.TelefonoEntrega;
            textBox3.Text = pedido.DireccionEntrega;
            textBox4.Text = pedido.Observaciones;
            textBox6.Text = pedido.MetodoPago;
            textBox5.Text = pedido.Estado;
            textBox7.Text = FormatearProductosConFacturacion(pedido.Productos, pedido.FechaHora);
        }

        private string FormatearProductosConFacturacion(string productosRaw, DateTime fechaPedido)
        {
            if (string.IsNullOrWhiteSpace(productosRaw))
                return "";

            var lineas = new List<string>();
            decimal totalPedido = 0;
            int productosEncontrados = 0;
            int productosNoEncontrados = 0;
            
            lineas.Add("═══════════ FACTURA HISTÓRICA ═══════════");
            lineas.Add($"📅 Fecha del pedido: {fechaPedido:dd/MM/yyyy HH:mm}");
            lineas.Add("");

            var productos = productosRaw.Split(',');
            
            foreach (var productoStr in productos)
            {
                var partes = productoStr.Split('x');
                if (partes.Length == 2)
                {
                    var cantidadStr = partes[0].Trim();
                    var nombreProducto = partes[1].Trim();
                    
                    if (int.TryParse(cantidadStr, out int cantidad))
                    {
                        try
                        {
                            // Buscar el producto por nombre en la base de datos
                            var productosEncontradosDB = _productoRepo.ObtenerTodosLosProductos()
                                .Where(p => p.Nombre.Equals(nombreProducto, StringComparison.OrdinalIgnoreCase))
                                .ToList();
                            
                            if (productosEncontradosDB.Any())
                            {
                                var producto = productosEncontradosDB.First();
                                decimal precioUnitario = producto.Precio;
                                decimal subtotal = cantidad * precioUnitario;
                                totalPedido += subtotal;
                                productosEncontrados++;
                                
                                lineas.Add($"• {cantidad} x {nombreProducto}");
                                lineas.Add($"  ${precioUnitario:F2} c/u → ${subtotal:F2}");
                                lineas.Add("");
                            }
                            else
                            {
                                // Producto no encontrado, mostrar sin precio
                                lineas.Add($"• {cantidad} x {nombreProducto}");
                                lineas.Add("  ⚠️ Producto no encontrado en catálogo actual");
                                lineas.Add("");
                                productosNoEncontrados++;
                            }
                        }
                        catch (Exception ex)
                        {
                            // En caso de error, mostrar sin precio
                            lineas.Add($"• {cantidad} x {nombreProducto}");
                            lineas.Add($"  ❌ Error al consultar precio: {ex.Message}");
                            lineas.Add("");
                            productosNoEncontrados++;
                        }
                    }
                    else
                    {
                        // Formato no válido, mostrar como está
                        lineas.Add($"• {productoStr}");
                        lineas.Add("  ⚠️ Formato de cantidad inválido");
                        lineas.Add("");
                        productosNoEncontrados++;
                    }
                }
                else
                {
                    // Formato no válido, mostrar como está
                    lineas.Add($"• {productoStr}");
                    lineas.Add("  ⚠️ Formato de producto inválido");
                    lineas.Add("");
                    productosNoEncontrados++;
                }
            }
            
            lineas.Add("═══════════════════════════════════════");
            
            if (productosEncontrados > 0)
            {
                lineas.Add($"💰 TOTAL ESTIMADO: ${totalPedido:F2}");
                lineas.Add("   (Basado en precios actuales)");
            }
            
            if (productosNoEncontrados > 0)
            {
                lineas.Add($"⚠️ {productosNoEncontrados} producto(s) sin precio actual");
                lineas.Add("   (Pueden haber cambiado desde el pedido)");
            }
            
            lineas.Add("═══════════════════════════════════════");

            return string.Join(Environment.NewLine, lineas);
        }

        private string FormatearProductos(string productosRaw)
        {
            if (string.IsNullOrWhiteSpace(productosRaw))
                return "";

            var lineas = productosRaw.Split(',')
                .Select(p =>
                {
                    var partes = p.Split('x');
                    if (partes.Length == 2)
                        return $"{partes[0].Trim()} x {partes[1].Trim()}";
                    return p;
                });

            return string.Join(Environment.NewLine, lineas);        }
    }
}
