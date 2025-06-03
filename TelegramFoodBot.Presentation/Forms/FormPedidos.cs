using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TelegramFoodBot.Data;
using TelegramFoodBot.Entities.Models;
using TelegramFoodBot.Business.Services;

namespace TelegramFoodBot.Presentation.Forms
{    public partial class FormPedidos : Form
    {        private readonly PedidoRepository _pedidoRepo = new PedidoRepository();
        private readonly ProductoRepository _productoRepo = new ProductoRepository();
        private List<Pedido> _pedidosActivos = new();
        private Pedido _pedidoSeleccionado;
        private readonly EnhancedTelegramService _telegramService = EnhancedTelegramService.Instance;public FormPedidos()
        {
            InitializeComponent();
            Load += FormPedidos_Load;

            // Asignar eventos a los botones de estado
            btPreparacion.Click += (s, e) => CambiarEstadoPedido("preparacion");
            btnCamino.Click += (s, e) => CambiarEstadoPedido("camino");
            btnTerminado.Click += (s, e) => CambiarEstadoPedido("terminado");
            
            // Agregar botones de confirmación de pago
            AgregarBotonesPago();
        }

        private void FormPedidos_Load(object sender, EventArgs e)
        {
            CargarPedidosActivos();
        }        private void CargarPedidosActivos()
        {
            panelPEDIDOSACTIVOS.Controls.Clear();
            _pedidosActivos = _pedidoRepo.ObtenerPedidosEnProceso();

            // También cargar pedidos pendientes de pago
            var pedidosPendientesPago = _pedidoRepo.ObtenerPedidosPendientesPago();
            _pedidosActivos.AddRange(pedidosPendientesPago);

            // ORDENAR PEDIDOS POR PRIORIDAD DE ESTADO
            // 1. Pendientes - PRIORIDAD MÁXIMA - Nuevos pedidos que requieren atención
            // 2. Pendiente Pago - Esperando confirmación de pago
            // 3. Aceptados - Listos para preparar
            // 4. Preparación - En cocina
            // 5. Camino - En delivery
            // 6. Terminados - Entregados

            var pedidosOrdenados = _pedidosActivos
                .OrderBy(p => GetPrioridadEstado(p.Estado))  // Primero por prioridad de estado
                .ThenBy(p => p.Id) // Luego por ID (orden cronológico)
                .ToList();

            int y = 10;
            string estadoAnterior = "";

            foreach (var pedido in pedidosOrdenados)
            {
                // Agregar separador visual entre grupos de estados
                if (pedido.Estado != estadoAnterior && !string.IsNullOrEmpty(estadoAnterior))
                {                    var separador = new Label
                    {
                        Text = $"──────── {pedido.Estado.ToUpper()} ────────",
                        Width = panelPEDIDOSACTIVOS.Width - 25,
                        Height = 25,
                        Location = new Point(10, y),
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        ForeColor = Color.Gray,
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.LightGray
                    };
                    panelPEDIDOSACTIVOS.Controls.Add(separador);
                    y += 35;
                }                var btn = new Button
                {
                    Text = $"🍕 Pedido #{pedido.Id} - {pedido.Estado}" + (pedido.PagoPendiente ? " (PAGO PENDIENTE)" : ""),
                    Width = panelPEDIDOSACTIVOS.Width - 25,
                    Height = 40,
                    Top = y,
                    Left = 10,
                    Tag = pedido,
                    BackColor = GetColorByStatePedido(pedido.Estado)
                };
                btn.Click += Pedido_Click;
                panelPEDIDOSACTIVOS.Controls.Add(btn);

                estadoAnterior = pedido.Estado;
                y += 50; // separa los botones
            }
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
            textBox1.Text = pedido.ClienteId.ToString();
            textBox2.Text = pedido.TelefonoEntrega;
            textBox3.Text = pedido.DireccionEntrega;
            textBox4.Text = pedido.Observaciones;
            textBox5.Text = pedido.Estado + (pedido.PagoPendiente ? " - PAGO PENDIENTE" : "");
            textBox6.Text = pedido.MetodoPago + (pedido.PagoConfirmado ? " ✅" : "");
            textBox7.Text = FormatearProductosConFacturacion(pedido.Productos);
            
            // Controlar visibilidad de botones según estado del pago
            if (pedido.PagoPendiente && !pedido.PagoConfirmado)
            {
                // Mostrar botones de confirmación de pago
                btnConfirmarPago.Visible = true;
                btnRechazarPago.Visible = true;
                
                // Ocultar botones de estado normales
                btPreparacion.Visible = false;
                btnCamino.Visible = false;
                btnTerminado.Visible = false;
            }
            else
            {
                // Mostrar botones de estado normales
                btnConfirmarPago.Visible = false;
                btnRechazarPago.Visible = false;
                btPreparacion.Visible = true;
                btnCamino.Visible = true;
                btnTerminado.Visible = true;
            }
        }        private string FormatearProductosConFacturacion(string productosRaw)
        {
            if (string.IsNullOrWhiteSpace(productosRaw))
                return "";

            var lineas = new List<string>();
            decimal totalPedido = 0;
            int productosEncontrados = 0;
            int productosNoEncontrados = 0;
            
            lineas.Add("═══════════ FACTURA DETALLADA ═══════════");
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
                                lineas.Add("  ⚠️ Precio no disponible en catálogo");
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
                lineas.Add($"💰 TOTAL DEL PEDIDO: ${totalPedido:F2}");
            }
            
            if (productosNoEncontrados > 0)
            {
                lineas.Add($"⚠️ {productosNoEncontrados} producto(s) sin precio");
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

            return string.Join(Environment.NewLine, lineas);
        }

        private void CambiarEstadoPedido(string nuevoEstado)
        {
            if (_pedidoSeleccionado == null)
            {
                MessageBox.Show("Seleccioná un pedido primero.");
                return;
            }

            _pedidoRepo.ActualizarEstadoPedido(_pedidoSeleccionado.Id, nuevoEstado);

            // Enviar mensaje según el estado
            long chatId = _pedidoSeleccionado.ClienteId;
            if (nuevoEstado == "preparacion")
            {
                string mensaje = $"👩‍🍳 ¡Hola! Tu pedido #{_pedidoSeleccionado.Id} está en preparación. Estamos cuidando cada detalle para que lo disfrutes al máximo. ¡Te avisaremos cuando salga rumbo a ti!";
                _telegramService.SendMessage(chatId, mensaje);
            }
            else if (nuevoEstado == "camino")
            {
                string mensaje = $"🚗🍔 ¡Buenas noticias! Tu pedido #{_pedidoSeleccionado.Id} ya va en camino. Prepárate para disfrutarlo pronto. Si tienes alguna pregunta, aquí estoy para ayudarte.";
                _telegramService.SendMessage(chatId, mensaje);
            }
            else if (nuevoEstado == "terminado")
            {
                string mensaje = $"🎉 ¡Tu pedido #{_pedidoSeleccionado.Id} ha sido entregado con éxito! Gracias por preferirnos. Esperamos que lo hayas disfrutado. ¿Quieres hacer otro pedido o tienes alguna sugerencia? ¡Estamos atentos!";
                _telegramService.SendMessage(chatId, mensaje);
            }

            MessageBox.Show($"Estado actualizado a '{nuevoEstado}'");

            CargarPedidosActivos();
            _pedidoSeleccionado.Estado = nuevoEstado;
            MostrarDetallesPedido(_pedidoSeleccionado);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null; // Quita el foco del botón
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            // No se usa, pero puede servir si querés permitir edición manual de productos
        }

        private void btPreparacion_Click(object sender, EventArgs e)
        {

        }

        private void FormPedidos_Load_1(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnTerminado_Click(object sender, EventArgs e)
        {

        }
        
        private Button btnConfirmarPago;
        private Button btnRechazarPago;
        
        private void AgregarBotonesPago()
        {
            // Crear botón de confirmar pago
            btnConfirmarPago = new Button
            {
                Text = "✅ Confirmar Pago",
                Width = 140,
                Height = 35,
                Left = 70,
                Top = 440,
                BackColor = Color.LightGreen,
                Visible = false
            };
            btnConfirmarPago.Click += BtnConfirmarPago_Click;
            
            // Crear botón de rechazar pago
            btnRechazarPago = new Button
            {
                Text = "❌ Rechazar Pago", 
                Width = 140,
                Height = 35,
                Left = 220,
                Top = 440,
                BackColor = Color.LightCoral,
                Visible = false
            };
            btnRechazarPago.Click += BtnRechazarPago_Click;
            
            // Agregar botones al panel3            panel3.Controls.Add(btnConfirmarPago);
            panel3.Controls.Add(btnRechazarPago);
        }
        
        private void BtnConfirmarPago_Click(object sender, EventArgs e)
        {
            if (_pedidoSeleccionado == null || !_pedidoSeleccionado.PagoPendiente)
            {
                MessageBox.Show("No hay un pedido con pago pendiente seleccionado.");
                return;
            }
            
            var result = MessageBox.Show(
                $"¿Confirmar el pago del pedido #{_pedidoSeleccionado.Id}?",
                "Confirmar Pago",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
                
            if (result == DialogResult.Yes)
            {
                try
                {
                    _pedidoRepo.ConfirmarPagoPedido(_pedidoSeleccionado.Id, "Manual desde Panel");
                    
                    // Notificar al cliente (opcional)
                    // _telegramService?.SendMessage(_pedidoSeleccionado.ClienteId, 
                    //     $"✅ ¡Pago confirmado! Tu pedido #{_pedidoSeleccionado.Id} está en preparación.");
                      MessageBox.Show("Pago confirmado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarPedidosActivos(); // Recargar la lista
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al confirmar el pago: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void BtnRechazarPago_Click(object sender, EventArgs e)
        {
            if (_pedidoSeleccionado == null || !_pedidoSeleccionado.PagoPendiente)
            {
                MessageBox.Show("No hay un pedido con pago pendiente seleccionado.");
                return;
            }
            
            var result = MessageBox.Show(
                $"¿Rechazar el pago del pedido #{_pedidoSeleccionado.Id}? Esta acción cancelará el pedido.",
                "Rechazar Pago",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
                
            if (result == DialogResult.Yes)
            {
                try
                {
                    _pedidoRepo.RechazarPagoPedido(_pedidoSeleccionado.Id);
                    
                    // Notificar al cliente (opcional)
                    // _telegramService?.SendMessage(_pedidoSeleccionado.ClienteId, 
                    //     $"❌ El pago para tu pedido #{_pedidoSeleccionado.Id} ha sido rechazado. El pedido fue cancelado.");
                      MessageBox.Show("Pago rechazado y pedido cancelado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarPedidosActivos(); // Recargar la lista
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al rechazar el pago: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        /// <summary>
        /// Define la prioridad de los estados de pedidos para el ordenamiento
        /// Números menores = mayor prioridad (aparecen primero)
        /// </summary>
        private int GetPrioridadEstado(string estado)
        {
            return estado.ToLower() switch
            {
                "pendiente" => 1,        // PRIORIDAD MÁXIMA - Nuevos pedidos que requieren atención
                "pendientepago" => 2,    // Esperando confirmación de pago
                "aceptado" => 3,         // Pedidos aceptados, listos para preparar
                "preparacion" => 4,      // En cocina
                "camino" => 5,           // En delivery
                "terminado" => 6,        // Entregados
                _ => 7                   // Otros estados al final
            };
        }

        /// <summary>
        /// Define colores por estado de pedido para mejor visualización
        /// </summary>
        private Color GetColorByStatePedido(string estado)
        {
            return estado.ToLower() switch
            {
                "pendiente" => Color.LightCoral,     // Rojo suave - Urgente
                "pendientepago" => Color.LightYellow, // Amarillo - Esperando pago
                "aceptado" => Color.LightBlue,       // Azul - Aceptado
                "preparacion" => Color.LightGreen,   // Verde - En cocina
                "camino" => Color.Orange,            // Naranja - En delivery
                "terminado" => Color.LightGray,      // Gris - Completado
                _ => Color.Khaki                     // Color por defecto
            };
        }
    }
}

