using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TelegramFoodBot.Data;
using TelegramFoodBot.Entities.Models;
using TelegramFoodBot.Business.Services;
using TelegramFoodBot.Business.Interfaces;

namespace TelegramFoodBot.Presentation.Forms
{    public partial class FormInicio : Form
    {
        private readonly EnhancedTelegramService _telegramService;
        private PedidoRepository _repo = new PedidoRepository();
        private ReservaRepository _reservaRepo = new ReservaRepository();
        
        // Variables para notificaciones de reservas
        private int _nuevasReservas = 0;
        private Timer _parpadeoTimer;
        private int _parpadeoCount = 0;
        private Color _colorOriginalReservas;        public FormInicio()
        {
            _telegramService = EnhancedTelegramService.Instance;
            InitializeComponent();
            this.Load += FormInicio_Load;
            _telegramService.PedidoCreado += TelegramService_PedidoCreado;
            _telegramService.ReservaCreada += TelegramService_ReservaCreada;
            _telegramService.StartBot();
        }        private void TelegramService_PedidoCreado(object sender, PedidoEventArgs e)
        {
            // Actualizar el panel de pedidos en el hilo de la UI
            if (InvokeRequired)
            {
                Invoke(new Action(() => RefrescarPedidosPorEvento(e.Pedido)));
            }
            else
            {
                RefrescarPedidosPorEvento(e.Pedido);
            }
        }

        private void TelegramService_ReservaCreada(object sender, ReservaEventArgs e)
        {
            // Manejar notificación de nueva reserva en el hilo de la UI
            if (InvokeRequired)
            {
                Invoke(new Action(() => NotificarNuevaReserva(e.Reserva)));
            }
            else
            {
                NotificarNuevaReserva(e.Reserva);
            }
        }private void RefrescarPedidosPorEvento(Pedido pedido)
        {
            // Solo agregar el nuevo pedido si corresponde
            if (pedido.Estado != null && (pedido.Estado.ToLower() == "pendiente" || pedido.Estado.ToLower() == "pendientepago"))
            {
                var contenedor = CrearContenedorPedido(pedido);
                
                // Suspender el layout para evitar parpadeos
                panelPEDIDOS.SuspendLayout();
                
                // Agregar al INICIO del panel (como en un chat) usando SetChildIndex
                panelPEDIDOS.Controls.Add(contenedor);
                panelPEDIDOS.Controls.SetChildIndex(contenedor, 0);
                
                // Reanudar el layout
                panelPEDIDOS.ResumeLayout();
                
                // Auto-scroll hacia la parte superior para mostrar el nuevo pedido
                HacerScrollHaciaArriba(panelPEDIDOS);
                
                // Notificación visual opcional (parpadeo del contenedor)
                ResaltarNuevoPedido(contenedor);
            }
            // Si el pedido es "aceptado", "preparacion" o "camino", agregarlo/actualizarlo en los activos
            else if (pedido.Estado != null && (pedido.Estado.ToLower() == "aceptado" || 
                                              pedido.Estado.ToLower() == "preparacion" || 
                                              pedido.Estado.ToLower() == "camino"))
            {
                // Recargar toda la lista de pedidos activos para estar seguro
                CargarPedidosActivos();
            }
            // Si el pedido es "terminado", "rechazado" o "cancelado", refrescar ambos paneles
            else if (pedido.Estado != null && (pedido.Estado.ToLower() == "terminado" || 
                                              pedido.Estado.ToLower() == "rechazado" || 
                                              pedido.Estado.ToLower() == "cancelado"))
            {
                // Recargar ambos paneles para remover el pedido
                CargarPedidosPendientes();
                CargarPedidosActivos();
            }
        }/// <summary>
        /// Crea un contenedor de pedido reutilizable
        /// </summary>
        private Panel CrearContenedorPedido(Pedido pedido)
        {
            var contenedor = new Panel
            {
                Width = panelPEDIDOS.Width - 25, // Ancho del panel menos espacio para scroll
                Height = 100,
                Margin = new Padding(5),
                BackColor = pedido.PagoPendiente ? Color.LightYellow : Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Top // Importante: esto hace que se apilen verticalmente
            };            // Agregar timestamp para mostrar cuándo llegó el pedido
            var lblTime = new Label
            {
                Text = pedido.FechaHora.ToString("HH:mm:ss"),
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = false,
                Width = 60,
                Height = 15,
                Location = new Point(10, 65),
                TextAlign = ContentAlignment.MiddleLeft
            };
            contenedor.Controls.Add(lblTime);

            var lbl = new Label
            {
                Text = $"🧑 {pedido.ClienteId} | 📦 {pedido.Productos}",
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                Width = 800,
                Height = 25,
                Location = new Point(10, 10)
            };
            contenedor.Controls.Add(lbl);

            var lblPago = new Label
            {
                Text = $"💳 {pedido.MetodoPago}" + (pedido.PagoPendiente ? " - 🔴 PAGO PENDIENTE" : ""),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = pedido.PagoPendiente ? Color.Red : Color.Green,
                AutoSize = false,
                Width = 800,
                Height = 20,
                Location = new Point(10, 35)
            };
            contenedor.Controls.Add(lblPago);

            if (pedido.PagoPendiente && !pedido.PagoConfirmado)
            {
                var btnConfirmarPago = new Button
                {
                    Text = "✅ Confirmar Pago",
                    Width = 120,
                    Height = 35,
                    Location = new Point(820, 10),
                    Tag = pedido.Id,
                    BackColor = Color.LightGreen
                };
                btnConfirmarPago.Click += (s, e) => { ConfirmarPagoPedido(pedido.Id); };
                contenedor.Controls.Add(btnConfirmarPago);

                var btnRechazarPago = new Button
                {
                    Text = "❌ Rechazar Pago",
                    Width = 120,
                    Height = 35,
                    Location = new Point(950, 10),
                    Tag = pedido.Id,
                    BackColor = Color.LightCoral
                };
                btnRechazarPago.Click += (s, e) => { RechazarPagoPedido(pedido.Id); };
                contenedor.Controls.Add(btnRechazarPago);
            }
            else if (!pedido.PagoPendiente)
            {                var btnAceptar = new Button
                {
                    Text = "✅ Aceptar",
                    Width = 100,
                    Height = 30,
                    Location = new Point(850, 20),
                    Tag = pedido.Id
                };
                btnAceptar.Click += (s, e) =>
                {
                    _repo.ActualizarEstadoPedido(pedido.Id, "Aceptado");
                    
                    // Refrescar ambos paneles para mover el pedido de pendientes a activos
                    CargarPedidosPendientes();
                    CargarPedidosActivos();
                };
                contenedor.Controls.Add(btnAceptar);

                var btnRechazar = new Button
                {
                    Text = "❌ Rechazar",
                    Width = 100,
                    Height = 30,
                    Location = new Point(960, 20),
                    Tag = pedido.Id
                };
                btnRechazar.Click += (s, e) =>
                {
                    _repo.ActualizarEstadoPedido(pedido.Id, "Rechazado");
                    
                    // Refrescar solo panel de pendientes (el pedido desaparece)
                    CargarPedidosPendientes();
                };
                contenedor.Controls.Add(btnRechazar);
            }

            return contenedor;
                }        private void FormInicio_Load(object sender, EventArgs e)
        {
            CargarPedidosPendientes();
            CargarPedidosActivos();
            CargarReservasPendientes();
            CargarReservasActivas();
            
            // Configurar scroll mejorado para paneles
            ConfigurarScrollMejorado(panelPEDIDOS);
            ConfigurarScrollMejorado(panelPEDIDOSACTIVOS);
            ConfigurarScrollMejorado(panelRESERVAS);
            ConfigurarScrollMejorado(panelRESERACTIVAS); // Agregar scroll al panel correcto
              // Inicializar color original para notificaciones
            _colorOriginalReservas = panelRESERVASACTIV.BackColor;
            
            // Agregar evento click para marcar reservas como vistas
            panelRESERVASACTIV.Click += PanelReservasActiv_Click;
            titleRESERVASACTI.Click += PanelReservasActiv_Click;
        }

        /// <summary>
        /// Configura un scroll más suave y funcional para los paneles
        /// </summary>
        private void ConfigurarScrollMejorado(Panel panel)
        {
            panel.AutoScroll = true;
            panel.HorizontalScroll.Enabled = false;
            panel.HorizontalScroll.Visible = false;
            panel.VerticalScroll.Enabled = true;
            panel.VerticalScroll.Visible = true;
              // Configurar incrementos de scroll más suaves
            panel.VerticalScroll.SmallChange = 20;
            panel.VerticalScroll.LargeChange = 60;
        }        /// <summary>
        /// Hace scroll hacia arriba para mostrar el elemento más reciente
        /// </summary>
        private void HacerScrollHaciaArriba(Panel panel)
        {
            // Si hay controles, hacer scroll hacia la parte superior
            if (panel.Controls.Count > 0)
            {
                panel.AutoScrollPosition = new Point(0, 0);
                panel.ScrollControlIntoView(panel.Controls[0]);
            }
            panel.Invalidate();
        }

        /// <summary>
        /// Resalta visualmente un nuevo pedido con un efecto de parpadeo
        /// </summary>
        private async void ResaltarNuevoPedido(Panel contenedor)
        {
            var colorOriginal = contenedor.BackColor;
            var colorResaltado = Color.LightCyan;

            // Animación de resaltado
            for (int i = 0; i < 3; i++)
            {
                contenedor.BackColor = colorResaltado;
                await Task.Delay(200);
                contenedor.BackColor = colorOriginal;
                await Task.Delay(200);
            }
        }        private void CargarPedidosPendientes()
        {
            panelPEDIDOS.Controls.Clear();

            // Obtener pedidos pendientes normales y pedidos pendientes de pago
            var pedidosPendientes = _repo.ObtenerPedidosActivos().Where(p => p.Estado.ToLower() == "pendiente").ToList();
            var pedidosPendientesPago = _repo.ObtenerPedidosPendientesPago();

            // Combinar ambas listas y ordenar por fecha (más reciente primero, estilo chat)
            var todosPendientes = pedidosPendientes.Concat(pedidosPendientesPago)
                                                   .OrderBy(p => p.FechaHora) // Orden cronológico normal
                                                   .ToList();

            // Suspender layout para mejor performance
            panelPEDIDOS.SuspendLayout();

            // Agregar pedidos en orden cronológico para que con Dock.Top el más reciente quede arriba
            foreach (var p in todosPendientes)
            {
                var contenedor = CrearContenedorPedido(p);
                panelPEDIDOS.Controls.Add(contenedor);
            }

            // Reanudar layout
            panelPEDIDOS.ResumeLayout();

            // Auto-scroll hacia arriba para mostrar los pedidos más recientes
            HacerScrollHaciaArriba(panelPEDIDOS);
        }        private void CargarPedidosActivos()
        {
            panelPEDIDOSACTIVOS.Controls.Clear();
            
            // CORREGIDO: Usar ObtenerPedidosEnProceso() en lugar de ObtenerPedidosActivos()
            // para obtener pedidos que están en estados activos (aceptado, preparacion, camino)
            var pedidos = _repo.ObtenerPedidosEnProceso();

            // Ordenar los pedidos por fecha (más recientes primero)
            pedidos = pedidos.OrderByDescending(p => p.FechaHora).ToList();

            // Suspender layout para mejor performance
            panelPEDIDOSACTIVOS.SuspendLayout();            foreach (var p in pedidos)
            {
                var btn = new Button
                {
                    Text = $"🧑 {p.ClienteId} | 📦 {p.Productos} | 🟡 {p.Estado}",
                    Width = panelPEDIDOSACTIVOS.Width - 25, // Ancho del panel menos espacio para scroll
                    Height = 50,
                    Margin = new Padding(5),
                    BackColor = p.Estado.ToLower() == "aceptado" ? Color.LightGreen : 
                               p.Estado.ToLower() == "preparacion" ? Color.Khaki :
                               p.Estado.ToLower() == "camino" ? Color.LightBlue : Color.White,
                    Tag = p.Id,
                    Dock = DockStyle.Top // Para que se apilen verticalmente
                };

                // Agregar funcionalidad de click para cambiar estado
                btn.Click += (sender, e) =>
                {
                    var pedidoId = btn.Tag.ToString();
                    var pedidoActual = pedidos.FirstOrDefault(ped => ped.Id == pedidoId);
                    
                    if (pedidoActual != null)
                    {
                        MostrarMenuEstado(pedidoActual, btn);
                    }
                };

                panelPEDIDOSACTIVOS.Controls.Add(btn);
            }

            // Reanudar layout
            panelPEDIDOSACTIVOS.ResumeLayout();
        }

        private void CargarReservasPendientes()
        {
            panelRESERVAS.Controls.Clear();
            var reservas = _reservaRepo.ObtenerReservasActivas().Where(r => r.Estado.ToLower() == "pendiente").ToList();

            foreach (var r in reservas)
            {
                var contenedor = new Panel
                {
                    Width = 1220,
                    Height = 80,
                    Margin = new Padding(5),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                var lbl = new Label
                {
                    Text = $"🧑 {r.Cliente} | 👥 {r.CantidadPersonas} | ⏰ {r.Fecha:dd/MM} - {r.Hora}",
                    Font = new Font("Segoe UI", 10),
                    AutoSize = false,
                    Width = 800,
                    Height = 30,
                    Location = new Point(10, 10)
                };
                contenedor.Controls.Add(lbl);

                var btnAceptar = new Button
                {
                    Text = "✅ Confirmar",
                    Width = 100,
                    Height = 30,
                    Location = new Point(850, 10),
                    Tag = r.Id
                };
                btnAceptar.Click += (s, e) =>
                {
                    _reservaRepo.ActualizarEstado(r.Id, "Confirmada");
                    CargarReservasPendientes();
                    CargarReservasActivas();
                };
                contenedor.Controls.Add(btnAceptar);

                var btnCancelar = new Button
                {
                    Text = "❌ Cancelar",
                    Width = 100,
                    Height = 30,
                    Location = new Point(960, 10),
                    Tag = r.Id
                };
                btnCancelar.Click += (s, e) =>
                {
                    _reservaRepo.ActualizarEstado(r.Id, "Cancelada");
                    CargarReservasPendientes();
                    CargarReservasActivas();
                };
                contenedor.Controls.Add(btnCancelar);

                panelRESERVAS.Controls.Add(contenedor);
            }
        }        private void CargarReservasActivas()
        {
            panelRESERACTIVAS.Controls.Clear();
            var reservas = _reservaRepo.ObtenerReservasActivas().Where(r => r.Estado.ToLower() == "confirmada").ToList();

            // Ordenar las reservas por fecha (más recientes primero)
            reservas = reservas.OrderByDescending(r => r.Fecha).ToList();

            // Suspender layout para mejor performance
            panelRESERACTIVAS.SuspendLayout();

            foreach (var r in reservas)
            {
                var btn = new Button
                {
                    Text = $"🧑 {r.Cliente} | 👥 {r.CantidadPersonas} | ⏰ {r.Fecha:dd/MM} - {r.Hora}",
                    Width = panelRESERACTIVAS.Width - 25, // Ancho del panel menos espacio para scroll
                    Height = 50,
                    Margin = new Padding(5),
                    BackColor = Color.LightGreen,
                    Tag = r.Id,
                    Dock = DockStyle.Top // Para que se apilen verticalmente
                };

                panelRESERACTIVAS.Controls.Add(btn);
            }

            // Reanudar layout
            panelRESERACTIVAS.ResumeLayout();
        }

        private void titleRESERVASACTI_Click(object sender, EventArgs e)
        {

        }

        private void ConfirmarPagoPedido(string pedidoId)
        {
            try
            {
                var result = MessageBox.Show(
                    $"¿Confirmar el pago del pedido #{pedidoId}?",
                    "Confirmar Pago",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _repo.ConfirmarPagoPedido(pedidoId, "Manual desde Panel");

                    // Encontrar el pedido para notificar al cliente
                    var pedidosPendientes = _repo.ObtenerPedidosPendientesPago();
                    var pedido = pedidosPendientes.Find(p => p.Id == pedidoId);                    if (pedido != null)
                    {
                        // Notificar al cliente usando EnhancedTelegramService
                        var telegramService = EnhancedTelegramService.Instance;
                        telegramService.SendMessage(pedido.ClienteId,
                            $"✅ ¡Excelente! Hemos confirmado tu pago para el pedido #{pedidoId}. Tu pedido ahora está en preparación. ¡Te avisaremos cuando esté listo!");
                    }

                    MessageBox.Show("Pago confirmado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarPedidosPendientes();
                    CargarPedidosActivos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al confirmar el pago: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RechazarPagoPedido(string pedidoId)
        {
            try
            {
                var result = MessageBox.Show(
                    $"¿Rechazar el pago del pedido #{pedidoId}? Esta acción cancelará el pedido.",
                    "Rechazar Pago",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Encontrar el pedido para notificar al cliente antes de cancelarlo
                    var pedidosPendientes = _repo.ObtenerPedidosPendientesPago();
                    var pedido = pedidosPendientes.Find(p => p.Id == pedidoId);

                    _repo.RechazarPagoPedido(pedidoId);                    if (pedido != null)
                    {
                        // Notificar al cliente usando EnhancedTelegramService
                        var telegramService = EnhancedTelegramService.Instance;
                        telegramService.SendMessage(pedido.ClienteId,
                            $"❌ Lo sentimos, no pudimos confirmar el pago para tu pedido #{pedidoId}. El pedido ha sido cancelado. Si realizaste la transferencia, por favor contáctanos para resolver el inconveniente.");
                    }

                    MessageBox.Show("Pago rechazado y pedido cancelado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarPedidosPendientes();
                    CargarPedidosActivos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al rechazar el pago: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        /// <summary>
        /// Muestra un menú contextual para cambiar el estado de un pedido
        /// </summary>
        private void MostrarMenuEstado(Pedido pedido, Button btn)
        {
            var menu = new ContextMenuStrip();

            // Opciones según el estado actual
            if (pedido.Estado.ToLower() == "aceptado")
            {
                var itemPreparacion = new ToolStripMenuItem("🍳 Pasar a Preparación");
                itemPreparacion.Click += (s, e) => CambiarEstadoPedido(pedido.Id, "preparacion");
                menu.Items.Add(itemPreparacion);
            }
            else if (pedido.Estado.ToLower() == "preparacion")
            {
                var itemCamino = new ToolStripMenuItem("🚗 Enviar (En Camino)");
                itemCamino.Click += (s, e) => CambiarEstadoPedido(pedido.Id, "camino");
                menu.Items.Add(itemCamino);
            }
            else if (pedido.Estado.ToLower() == "camino")
            {
                var itemTerminado = new ToolStripMenuItem("✅ Marcar como Entregado");
                itemTerminado.Click += (s, e) => CambiarEstadoPedido(pedido.Id, "terminado");
                menu.Items.Add(itemTerminado);
            }

            // Opción para cancelar (siempre disponible)
            if (menu.Items.Count > 0)
            {
                menu.Items.Add(new ToolStripSeparator());
            }
            
            var itemCancelar = new ToolStripMenuItem("❌ Cancelar Pedido");
            itemCancelar.Click += (s, e) => 
            {
                var result = MessageBox.Show($"¿Estás seguro de cancelar el pedido #{pedido.Id}?", 
                                           "Confirmar Cancelación", 
                                           MessageBoxButtons.YesNo, 
                                           MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    CambiarEstadoPedido(pedido.Id, "cancelado");
                }
            };
            menu.Items.Add(itemCancelar);

            // Mostrar el menú en la posición del botón
            menu.Show(btn, new Point(0, btn.Height));
        }

        /// <summary>
        /// Cambia el estado de un pedido y actualiza la interfaz
        /// </summary>
        private void CambiarEstadoPedido(string pedidoId, string nuevoEstado)
        {
            try
            {
                _repo.ActualizarEstadoPedido(pedidoId, nuevoEstado);

                // Enviar notificación al cliente si es necesario
                EnviarNotificacionCambioEstado(pedidoId, nuevoEstado);

                // Refrescar la interfaz
                CargarPedidosActivos();
                
                // Si el pedido fue terminado o cancelado, también refrescar pendientes por si acaso
                if (nuevoEstado.ToLower() == "terminado" || nuevoEstado.ToLower() == "cancelado")
                {
                    CargarPedidosPendientes();
                }

                MessageBox.Show($"Pedido #{pedidoId} actualizado a: {nuevoEstado}", 
                              "Estado Actualizado", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar el pedido: {ex.Message}", 
                              "Error", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Envía notificación al cliente sobre el cambio de estado
        /// </summary>
        private void EnviarNotificacionCambioEstado(string pedidoId, string nuevoEstado)
        {
            try
            {
                // Obtener información del pedido para encontrar el ClienteId
                var pedidos = _repo.ObtenerPedidosEnProceso();
                var pedido = pedidos.FirstOrDefault(p => p.Id == pedidoId);

                if (pedido != null)
                {
                    long chatId = pedido.ClienteId;
                    string mensaje = "";

                    switch (nuevoEstado.ToLower())
                    {
                        case "preparacion":
                            mensaje = $"👩‍🍳 ¡Tu pedido #{pedidoId} está en preparación! Estamos cuidando cada detalle para que lo disfrutes al máximo.";
                            break;
                        case "camino":
                            mensaje = $"🚗 ¡Tu pedido #{pedidoId} va en camino! Prepárate para disfrutarlo pronto.";
                            break;
                        case "terminado":
                            mensaje = $"🎉 ¡Tu pedido #{pedidoId} ha sido entregado! Gracias por preferirnos. ¡Esperamos que lo disfrutes!";
                            break;
                        case "cancelado":
                            mensaje = $"❌ Tu pedido #{pedidoId} ha sido cancelado. Si tienes alguna pregunta, no dudes en contactarnos.";
                            break;
                    }

                    if (!string.IsNullOrEmpty(mensaje))
                    {
                        _telegramService.SendMessage(chatId, mensaje);
                    }
                }
            }            catch (Exception ex)
            {
                // Si falla el envío del mensaje, no interrumpir el proceso principal
                System.Diagnostics.Debug.WriteLine($"Error enviando notificación: {ex.Message}");
            }
        }

        /// <summary>
        /// Notifica al restaurante cuando llega una nueva reserva
        /// Implementa badge + sonido + parpadeo suave
        /// </summary>
        private void NotificarNuevaReserva(Reserva reserva)
        {
            try
            {
                // 1. Incrementar contador de nuevas reservas
                _nuevasReservas++;

                // 2. Actualizar badge en el título de reservas
                ActualizarBadgeReservas();

                // 3. Reproducir sonido discreto
                ReproducirSonidoNotificacion();

                // 4. Iniciar parpadeo suave del panel de reservas
                IniciarParpadeoReservas();

                // 5. Refrescar la lista de reservas para mostrar la nueva
                RefrescarReservasActivas();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en NotificarNuevaReserva: {ex.Message}");
            }
        }        /// <summary>
        /// Actualiza el badge con el contador de nuevas reservas
        /// </summary>
        private void ActualizarBadgeReservas()
        {
            if (_nuevasReservas > 0)
            {
                // Actualizar el texto del título para incluir el badge con texto descriptivo
                titleRESERVASACTI.Text = $"RESERVAS ACTIVAS (solicitudes nuevas {_nuevasReservas})";
                titleRESERVASACTI.ForeColor = Color.Red;
                titleRESERVASACTI.Font = new Font(titleRESERVASACTI.Font, FontStyle.Bold);
            }
            else
            {
                // Restaurar el texto original
                titleRESERVASACTI.Text = "RESERVAS ACTIVAS";
                titleRESERVASACTI.ForeColor = Color.Black;
                titleRESERVASACTI.Font = new Font(titleRESERVASACTI.Font, FontStyle.Bold);
            }
        }

        /// <summary>
        /// Reproduce un sonido discreto para la notificación
        /// </summary>
        private void ReproducirSonidoNotificacion()
        {
            try
            {
                // Usar el sonido del sistema (beep) como notificación discreta
                System.Console.Beep(800, 200); // Frecuencia 800Hz, duración 200ms
            }
            catch (Exception ex)
            {
                // Si falla el sonido, continuar sin interrumpir
                System.Diagnostics.Debug.WriteLine($"Error reproduciendo sonido: {ex.Message}");
            }
        }

        /// <summary>
        /// Inicia el parpadeo suave del panel de reservas (3 veces)
        /// </summary>
        private void IniciarParpadeoReservas()
        {
            try
            {
                // Guardar el color original si no se ha guardado
                if (_colorOriginalReservas == Color.Empty)
                {
                    _colorOriginalReservas = panelRESERVASACTIV.BackColor;
                }

                // Reiniciar el contador de parpadeo
                _parpadeoCount = 0;

                // Detener timer anterior si existe
                if (_parpadeoTimer != null)
                {
                    _parpadeoTimer.Stop();
                    _parpadeoTimer.Dispose();
                }

                // Crear nuevo timer para el parpadeo
                _parpadeoTimer = new Timer();
                _parpadeoTimer.Interval = 300; // Parpadeo cada 300ms
                _parpadeoTimer.Tick += ParpadeoTimer_Tick;
                _parpadeoTimer.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error iniciando parpadeo: {ex.Message}");
            }
        }

        /// <summary>
        /// Maneja el evento de tick del timer de parpadeo
        /// </summary>
        private void ParpadeoTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_parpadeoCount < 6) // 3 parpadeos = 6 cambios de color
                {
                    // Alternar entre color original y color de notificación
                    if (_parpadeoCount % 2 == 0)
                    {
                        // Cambiar a color de notificación (amarillo suave)
                        panelRESERVASACTIV.BackColor = Color.LightYellow;
                    }
                    else
                    {
                        // Volver al color original
                        panelRESERVASACTIV.BackColor = _colorOriginalReservas;
                    }
                    _parpadeoCount++;
                }
                else
                {
                    // Finalizar parpadeo y restaurar color original
                    panelRESERVASACTIV.BackColor = _colorOriginalReservas;
                    _parpadeoTimer.Stop();
                    _parpadeoTimer.Dispose();
                    _parpadeoTimer = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en parpadeo timer: {ex.Message}");
            }
        }

        /// <summary>
        /// Refresca la lista de reservas activas para mostrar las nuevas
        /// </summary>
        private void RefrescarReservasActivas()
        {
            try
            {
                // Recargar las reservas activas
                CargarReservasActivas();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refrescando reservas: {ex.Message}");
            }
        }        /// <summary>
        /// Reinicia el contador de nuevas reservas (llamar cuando el usuario vea las reservas)
        /// </summary>
        public void MarcarReservasComoVistas()
        {
            _nuevasReservas = 0;
            ActualizarBadgeReservas();
        }

        /// <summary>
        /// Maneja el click en el panel de reservas para marcar como vistas
        /// </summary>
        private void PanelReservasActiv_Click(object sender, EventArgs e)
        {
            MarcarReservasComoVistas();
        }
    }
}
