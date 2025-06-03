using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TelegramFoodBot.Data;
using TelegramFoodBot.Entities.Models;
using TelegramFoodBot.Business.Services;

namespace TelegramFoodBot.Presentation.Forms
{
    public partial class FormReservas : Form
    {
        public FormReservas()
        {
            InitializeComponent();
        }        private ReservaRepository _reservaRepo = new ReservaRepository();
        private EnhancedTelegramService _telegramService = EnhancedTelegramService.Instance;
        private List<Reserva> _reservas = new List<Reserva>();

        private void FormReservas_Load(object sender, EventArgs e)
        {
            CargarReservas();
        }        private void CargarReservas()
        {
            panelPEDIDOSACTIVOS.Controls.Clear();
            _reservas = _reservaRepo.ObtenerReservasActivas();

            // ORDENAR RESERVAS POR PRIORIDAD DE ESTADO Y FECHA
            // 1. Solicitudes (pendientes) - PRIORIDAD ALTA - más recientes primero
            // 2. Aceptadas (esperando pago) - ordenadas por fecha de reserva
            // 3. Confirmadas (pagadas) - ordenadas por fecha de reserva  
            // 4. Otros estados - ordenados por fecha

            var reservasOrdenadas = _reservas
                .OrderBy(r => GetPrioridadEstado(r.Estado))  // Primero por prioridad de estado
                .ThenByDescending(r => r.Estado.ToLower() == "solicitud" ? r.FechaCreacion : DateTime.MinValue) // Solicitudes más recientes primero
                .ThenBy(r => r.Estado.ToLower() != "solicitud" ? r.Fecha : DateTime.MaxValue) // Otros estados por fecha de reserva
                .ThenBy(r => r.Hora) // Luego por hora
                .ToList();

            int y = 10;
            string estadoAnterior = "";

            foreach (var r in reservasOrdenadas)
            {
                // Agregar separador visual entre grupos de estados
                if (r.Estado != estadoAnterior && !string.IsNullOrEmpty(estadoAnterior))
                {
                    var separador = new Label
                    {
                        Text = $"──────── {r.Estado.ToUpper()} ────────",
                        Width = 550,
                        Height = 25,
                        Location = new Point(10, y),
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        ForeColor = Color.Gray,
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.LightGray
                    };
                    panelPEDIDOSACTIVOS.Controls.Add(separador);
                    y += 35;
                }

                var btn = new Button
                {
                    Text = $"📆 {r.Fecha:dd/MM} - {r.Hora} | {r.Cliente} ({r.CantidadPersonas} personas) - {r.Estado}",
                    Width = 550,
                    Height = 50,
                    BackColor = GetColorByState(r.Estado),
                    Location = new Point(10, y),
                    Tag = r
                };

                btn.Click += (s, e) => MostrarDetalles((Reserva)btn.Tag);
                panelPEDIDOSACTIVOS.Controls.Add(btn);

                estadoAnterior = r.Estado;
                y += 60; // separa los botones
            }
        }

        /// <summary>
        /// Define la prioridad de los estados para el ordenamiento
        /// Números menores = mayor prioridad (aparecen primero)
        /// </summary>
        private int GetPrioridadEstado(string estado)
        {
            return estado.ToLower() switch
            {
                "solicitud" => 1,    // PRIORIDAD MÁXIMA - Nuevas solicitudes arriba
                "pendiente" => 1,    // Alias de solicitud
                "aceptada" => 2,     // Esperando pago
                "confirmada" => 3,   // Pagadas y confirmadas
                "completada" => 4,   // Ya atendidas
                "rechazada" => 5,    // Rechazadas
                "cancelada" => 6,    // Canceladas
                "noshow" => 7,       // No se presentaron
                _ => 8               // Otros estados al final
            };
        }

        private Color GetColorByState(string estado)
        {
            return estado.ToLower() switch
            {
                "solicitud" => Color.LightBlue,     // Nueva solicitud
                "aceptada" => Color.LightYellow,    // Esperando pago
                "confirmada" => Color.LightGreen,   // Pago confirmado
                "completada" => Color.LightGray,    // Cliente asistió
                "rechazada" => Color.LightCoral,    // Rechazada
                "cancelada" => Color.LightPink,     // Cancelada
                "noshow" => Color.Orange,           // No se presentó
                _ => Color.Khaki
            };
        }        private void MostrarDetalles(Reserva reserva)
        {
            panel3.Controls.Clear();

            var lbl = new Label
            {
                Text = $"Cliente: {reserva.Cliente}\n" +
                       $"Teléfono: {reserva.Telefono}\n" +
                       $"Personas: {reserva.CantidadPersonas}\n" +
                       $"Fecha: {reserva.Fecha:dd/MM/yyyy}\n" +
                       $"Hora: {reserva.Hora}\n" +
                       $"Estado: {reserva.Estado}\n" +
                       $"Obs: {reserva.Observaciones}\n" +
                       $"Monto Seguro: ${reserva.MontoSeguro:N0}\n" +
                       $"Seguro Pagado: {(reserva.SeguroPagado ? "Sí" : "No")}\n" +
                       $"Cliente Asistió: {(reserva.ClienteAsistio ? "Sí" : "No")}\n" +
                       $"Seguro Reembolsado: {(reserva.SeguroReembolsado ? "Sí" : "No")}",
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 10)
            };

            panel3.Controls.Add(lbl);

            // Agregar botones de acción según el estado
            AgregarBotonesAccion(reserva);
        }

        private void AgregarBotonesAccion(Reserva reserva)
        {
            int yPos = 250;

            switch (reserva.Estado.ToLower())
            {
                case "solicitud":
                    // Botones para Aceptar/Rechazar
                    var btnAceptar = new Button
                    {
                        Text = "✅ Aceptar Reserva",
                        Width = 150,
                        Height = 40,
                        BackColor = Color.Green,
                        ForeColor = Color.White,
                        Location = new Point(10, yPos),
                        Tag = reserva
                    };
                    btnAceptar.Click += (s, e) => AceptarReserva((Reserva)((Button)s).Tag);

                    var btnRechazar = new Button
                    {
                        Text = "❌ Rechazar",
                        Width = 150,
                        Height = 40,
                        BackColor = Color.Red,
                        ForeColor = Color.White,
                        Location = new Point(170, yPos),
                        Tag = reserva
                    };
                    btnRechazar.Click += (s, e) => RechazarReserva((Reserva)((Button)s).Tag);

                    panel3.Controls.Add(btnAceptar);
                    panel3.Controls.Add(btnRechazar);
                    break;

                case "aceptada":
                    // Botón para confirmar pago manualmente
                    var btnConfirmarPago = new Button
                    {
                        Text = "💳 Confirmar Pago",
                        Width = 150,
                        Height = 40,
                        BackColor = Color.Orange,
                        ForeColor = Color.White,
                        Location = new Point(10, yPos),
                        Tag = reserva
                    };
                    btnConfirmarPago.Click += (s, e) => ConfirmarPago((Reserva)((Button)s).Tag);
                    panel3.Controls.Add(btnConfirmarPago);
                    break;

                case "confirmada":
                    // Botones para marcar asistencia
                    var btnAsistio = new Button
                    {
                        Text = "✅ Cliente Asistió",
                        Width = 150,
                        Height = 40,
                        BackColor = Color.Green,
                        ForeColor = Color.White,
                        Location = new Point(10, yPos),
                        Tag = reserva
                    };
                    btnAsistio.Click += (s, e) => MarcarAsistencia((Reserva)((Button)s).Tag, true);

                    var btnNoAsistio = new Button
                    {
                        Text = "❌ No Show",
                        Width = 150,
                        Height = 40,
                        BackColor = Color.Red,
                        ForeColor = Color.White,
                        Location = new Point(170, yPos),
                        Tag = reserva
                    };
                    btnNoAsistio.Click += (s, e) => MarcarAsistencia((Reserva)((Button)s).Tag, false);

                    panel3.Controls.Add(btnAsistio);
                    panel3.Controls.Add(btnNoAsistio);
                    break;

                case "completada":
                    if (reserva.SeguroPagado && !reserva.SeguroReembolsado)
                    {
                        var btnReembolsar = new Button
                        {
                            Text = "💰 Reembolsar Seguro",
                            Width = 150,
                            Height = 40,
                            BackColor = Color.Blue,
                            ForeColor = Color.White,
                            Location = new Point(10, yPos),
                            Tag = reserva
                        };
                        btnReembolsar.Click += (s, e) => ReembolsarSeguro((Reserva)((Button)s).Tag);
                        panel3.Controls.Add(btnReembolsar);
                    }
                    break;
            }
        }        private void AceptarReserva(Reserva reserva)
        {
            try
            {
                // Actualizar el estado de la reserva a "Aceptada"
                _reservaRepo.ActualizarEstado(reserva.Id, "Aceptada");
                
                // Notificar al cliente sobre la aprobación y el pago del seguro requerido
                string mensaje = $"🎉 ¡Tu reserva ha sido ACEPTADA!\n\n" +
                               $"📅 Fecha: {reserva.Fecha:dd/MM/yyyy}\n" +
                               $"⏰ Hora: {reserva.Hora}\n" +
                               $"👥 Personas: {reserva.CantidadPersonas}\n\n" +
                               $"💰 Para confirmar tu reserva, debes pagar un seguro de ${reserva.MontoSeguro:N0} COP.\n\n" +
                               $"Este seguro será:\n" +
                               $"✅ Reembolsado completamente si asistes\n" +
                               $"❌ Retenido si no te presentas\n\n" +
                               $"Por favor, realiza el pago y notifícanos para confirmar tu reserva.";
                
                _telegramService.SendMessage(reserva.ClienteId, mensaje);
                
                MessageBox.Show($"Reserva aceptada y cliente notificado sobre el pago del seguro de ${reserva.MontoSeguro:N0}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aceptar reserva: {ex.Message}");
            }
            
            CargarReservas();
        }

        private void RechazarReserva(Reserva reserva)
        {
            try
            {
                // Actualizar el estado de la reserva a "Rechazada"
                _reservaRepo.ActualizarEstado(reserva.Id, "Rechazada");
                
                // Notificar al cliente sobre el rechazo
                string mensaje = $"❌ Lamentablemente tu reserva ha sido RECHAZADA.\n\n" +
                               $"📅 Fecha solicitada: {reserva.Fecha:dd/MM/yyyy}\n" +
                               $"⏰ Hora solicitada: {reserva.Hora}\n" +
                               $"👥 Personas: {reserva.CantidadPersonas}\n\n" +
                               $"Motivo: No disponibilidad en la fecha/hora solicitada.\n\n" +
                               $"Te invitamos a intentar con otra fecha y hora. ¡Gracias por tu comprensión!";
                
                _telegramService.SendMessage(reserva.ClienteId, mensaje);
                
                MessageBox.Show("Reserva rechazada y cliente notificado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al rechazar reserva: {ex.Message}");
            }
            
            CargarReservas();
        }        private void ConfirmarPago(Reserva reserva)
        {
            try
            {
                // Marcar pago como confirmado y cambiar estado a "Confirmada"
                _reservaRepo.ActualizarPagoSeguro(reserva.Id, reserva.MontoSeguro, "Manual");
                
                // Notificar al cliente sobre la confirmación
                string mensaje = $"✅ ¡PAGO CONFIRMADO!\n\n" +
                               $"Tu reserva está ahora CONFIRMADA:\n\n" +
                               $"📅 Fecha: {reserva.Fecha:dd/MM/yyyy}\n" +
                               $"⏰ Hora: {reserva.Hora}\n" +
                               $"👥 Personas: {reserva.CantidadPersonas}\n\n" +
                               $"💰 Seguro pagado: ${reserva.MontoSeguro:N0} COP\n\n" +
                               $"🎯 ¡Te esperamos! El seguro será reembolsado o descontado de tu consumo cuando llegues.\n\n" +
                               $"¡Gracias por elegirnos!";
                
                _telegramService.SendMessage(reserva.ClienteId, mensaje);
                
                MessageBox.Show("Pago confirmado y cliente notificado. Reserva confirmada.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al confirmar pago: {ex.Message}");
            }
            
            CargarReservas();
        }        private void MarcarAsistencia(Reserva reserva, bool asistio)
        {
            try
            {
                // Registrar asistencia en la base de datos
                _reservaRepo.RegistrarAsistencia(reserva.Id, asistio);
                
                string mensaje;
                if (asistio)
                {
                    mensaje = $"✅ ¡Gracias por venir!\n\n" +
                            $"Tu asistencia ha sido registrada:\n" +
                            $"📅 {reserva.Fecha:dd/MM/yyyy} a las {reserva.Hora}\n\n" +
                            $"💰 Tu seguro de ${reserva.MontoSeguro:N0} COP será reembolsado o descontado de tu consumo.\n\n" +
                            $"¡Esperamos que hayas disfrutado tu experiencia con nosotros!";
                    
                    MessageBox.Show("Cliente marcado como asistido. Seguro disponible para reembolso.");
                }
                else
                {
                    mensaje = $"❌ Reserva marcada como No Show\n\n" +
                            $"No te presentaste a tu reserva:\n" +
                            $"📅 {reserva.Fecha:dd/MM/yyyy} a las {reserva.Hora}\n\n" +
                            $"💰 El seguro de ${reserva.MontoSeguro:N0} COP ha sido retenido según nuestras políticas.\n\n" +
                            $"Te esperamos en una próxima oportunidad.";
                    
                    MessageBox.Show("Cliente marcado como No Show. Seguro retenido.");
                }
                
                _telegramService.SendMessage(reserva.ClienteId, mensaje);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar asistencia: {ex.Message}");
            }
            
            CargarReservas();
        }        private void ReembolsarSeguro(Reserva reserva)
        {
            try
            {
                // Procesar reembolso en la base de datos
                _reservaRepo.ProcesarReembolsoSeguro(reserva.Id);
                
                // Notificar al cliente sobre el reembolso
                string mensaje = $"💰 ¡REEMBOLSO PROCESADO!\n\n" +
                               $"Tu seguro de ${reserva.MontoSeguro:N0} COP ha sido reembolsado exitosamente.\n\n" +
                               $"📅 Reserva: {reserva.Fecha:dd/MM/yyyy} a las {reserva.Hora}\n\n" +
                               $"El dinero será devuelto al método de pago original en los próximos días hábiles.\n\n" +
                               $"¡Gracias por cumplir con tu reserva!";
                
                _telegramService.SendMessage(reserva.ClienteId, mensaje);
                
                MessageBox.Show("Seguro reembolsado y cliente notificado.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar reembolso: {ex.Message}");
            }
            
            CargarReservas();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
