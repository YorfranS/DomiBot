using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFoodBot.Data;
using TelegramFoodBot.Entities.Models;
using TelegramFoodBot.Business.Services;
using AppMessage = TelegramFoodBot.Entities.Models.Message;
using TelegramMessage = Telegram.Bot.Types.Message;

namespace TelegramFoodBot.Business.Commands
{    public class ComandoReserva
    {
        private readonly TelegramBotClient _bot;
        private readonly Action<AppMessage> _onMessage;
        private readonly Dictionary<long, ReservaEnProceso> _reservasEnProceso = new();
        
        // Constante para el monto del seguro
        private const decimal MONTO_SEGURO = 10000m; // $10,000 COP

        public ComandoReserva(TelegramBotClient bot, Action<AppMessage> onMessage)
        {
            _bot = bot;
            _onMessage = onMessage;
        }

        public bool EnReserva(long clientId) => _reservasEnProceso.ContainsKey(clientId);

        public async Task Ejecutar(TelegramMessage message)
        {
            if (message.From == null)
                return;
            long clientId = message.From.Id;
            string texto = message.Text?.Trim() ?? "";            if (!_reservasEnProceso.ContainsKey(clientId))
            {
                _reservasEnProceso[clientId] = new ReservaEnProceso
                {
                    Cliente = message.From.FirstName,
                    Telefono = "Desconocido",
                    PasoActual = PasoReserva.Cantidad
                };

                // Mostrar botones de cantidad predefinida
                var tecladoCantidad = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("1", "cantidad_1"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("2", "cantidad_2"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("3", "cantidad_3")
                    },
                    new[]
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("4", "cantidad_4"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("5", "cantidad_5"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("Más", "cantidad_mas")
                    }
                });
                
                await Responder("👥 ¿Para cuántas personas deseas reservar?", message, tecladoCantidad);
                return;
            }

            var reserva = _reservasEnProceso[clientId];

            // Manejo especial para botones de cantidad predefinida
            if (reserva.PasoActual == PasoReserva.Cantidad && texto.StartsWith("cantidad_"))
            {
                var cantidadSeleccionada = texto.Substring("cantidad_".Length);
                
                if (cantidadSeleccionada == "mas")
                {
                    // Si selecciona "Más", pedir que digite la cantidad
                    reserva.PasoActual = PasoReserva.CantidadPersonalizada;
                    await Responder("👥 Por favor, digite la cantidad de personas:", message);
                    return;
                }
                else if (int.TryParse(cantidadSeleccionada, out int cantidadPredefinida) && cantidadPredefinida > 0)
                {
                    reserva.CantidadPersonas = cantidadPredefinida;
                    reserva.PasoActual = PasoReserva.Fecha;
                    
                    // Mostrar botones para fechas predefinidas
                    var tecladoFecha = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📅 Hoy", "fecha_hoy"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📅 Mañana", "fecha_manana")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📝 Otra fecha", "fecha_otra")
                        }
                    });
                    await Responder("📅 ¿Para qué fecha deseas reservar?", message, tecladoFecha);
                    return;
                }
            }            // Manejo especial para botones de observación
            if (reserva.PasoActual == PasoReserva.Observaciones && (texto == "observacion_si" || texto == "observacion_no"))
            {
                if (texto == "observacion_no")
                {
                    reserva.Observaciones = "";
                    reserva.PasoActual = PasoReserva.Confirmacion;
                }
                else if (texto == "observacion_si")
                {
                    await Responder("📝 Por favor, escribe tu observación o preferencia:", message);
                    return;
                }
            }

            // Manejo especial para botones de fecha predefinida
            if (reserva.PasoActual == PasoReserva.Fecha && (texto == "fecha_hoy" || texto == "fecha_manana" || texto == "fecha_otra"))
            {
                if (texto == "fecha_hoy")
                {
                    reserva.Fecha = DateTime.Today;
                    reserva.PasoActual = PasoReserva.Hora;
                    await Responder("⏰ ¿A qué hora? (Ej: 19:30)", message);
                    return;
                }
                else if (texto == "fecha_manana")
                {
                    reserva.Fecha = DateTime.Today.AddDays(1);
                    reserva.PasoActual = PasoReserva.Hora;
                    await Responder("⏰ ¿A qué hora? (Ej: 19:30)", message);
                    return;
                }
                else if (texto == "fecha_otra")
                {
                    await Responder("📅 Por favor, digita la fecha en formato DD/MM/AAAA:", message);
                    return;
                }
            }

            switch (reserva.PasoActual)
            {
                case PasoReserva.Cantidad:
                    if (!int.TryParse(texto, out int cantidad) || cantidad <= 0)
                    {
                        await Responder("❌ Ingresa un número válido de personas.", message);
                        return;
                    }                    reserva.CantidadPersonas = cantidad;
                    reserva.PasoActual = PasoReserva.Fecha;
                    
                    // Mostrar botones para fechas predefinidas
                    var tecladoFecha = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📅 Hoy", "fecha_hoy"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📅 Mañana", "fecha_manana")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📝 Otra fecha", "fecha_otra")
                        }
                    });
                    await Responder("📅 ¿Para qué fecha deseas reservar?", message, tecladoFecha);
                    break;

                case PasoReserva.CantidadPersonalizada:
                    if (!int.TryParse(texto, out int cantidadPersonalizada) || cantidadPersonalizada <= 0)
                    {
                        await Responder("❌ Ingresa un número válido de personas.", message);
                        return;
                    }

                    reserva.CantidadPersonas = cantidadPersonalizada;
                    reserva.PasoActual = PasoReserva.Fecha;
                    
                    // Mostrar botones para fechas predefinidas
                    var tecladoFechaPersonalizada = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📅 Hoy", "fecha_hoy"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📅 Mañana", "fecha_manana")
                        },
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📝 Otra fecha", "fecha_otra")
                        }
                    });
                    await Responder("📅 ¿Para qué fecha deseas reservar?", message, tecladoFechaPersonalizada);
                    break;

                case PasoReserva.Fecha:
                    if (!DateTime.TryParseExact(texto, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
                    {
                        await Responder("❌ Fecha inválida. Usa el formato DD/MM/AAAA.", message);
                        return;
                    }

                    reserva.Fecha = fecha;
                    reserva.PasoActual = PasoReserva.Hora;
                    await Responder("⏰ ¿A qué hora? (Ej: 19:30)", message);
                    break;

                case PasoReserva.Hora:
                    reserva.Hora = texto;
                    reserva.PasoActual = PasoReserva.Observaciones;
                    // Mostrar botones para observación
                    var tecladoObs = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Sí", "observacion_si"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ No", "observacion_no")
                        }
                    });
                    await Responder("📝 ¿Deseas agregar alguna observación o preferencia?", message, tecladoObs);
                    break;
                case PasoReserva.Observaciones:
                    // Si viene de botón y ya se procesó, continuar
                    if (string.IsNullOrWhiteSpace(reserva.Observaciones) && (texto == "observacion_no"))
                    {
                        // Ya se procesó arriba
                    }
                    else if (texto != "observacion_si" && texto != "observacion_no")
                    {
                        reserva.Observaciones = texto.Equals("no", StringComparison.OrdinalIgnoreCase) ? "" : texto;
                    }
                    reserva.PasoActual = PasoReserva.Confirmacion;
                    // Crear mensaje de confirmación con resumen
                    string resumenReserva = $"*Resumen de tu reserva:*\n\n" +
                                          $"👥 Personas: {reserva.CantidadPersonas}\n" +
                                          $"📅 Fecha: {reserva.Fecha.ToString("dd/MM/yyyy")}\n" +
                                          $"⏰ Hora: {reserva.Hora}\n" +
                                          $"📝 Observaciones: {(string.IsNullOrWhiteSpace(reserva.Observaciones) ? "Ninguna" : reserva.Observaciones)}\n\n" +
                                          $"💰 Seguro requerido: ${MONTO_SEGURO:N0} COP\n\n" +
                                          $"¿Confirmas esta solicitud de reserva?";
                    // Botones de confirmación
                    var teclado = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Sí, confirmar", "reserva_confirmar"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ No, cancelar", "reserva_cancelar")
                        }
                    });
                    await Responder(resumenReserva, message, teclado);
                    break;
                    
                case PasoReserva.Confirmacion:
                    // Verificar si confirma o cancela
                    if (texto.Equals("no", StringComparison.OrdinalIgnoreCase) || texto == "reserva_cancelar")
                    {
                        _reservasEnProceso.Remove(clientId);
                        await Responder("❌ Solicitud de reserva cancelada. ¿Puedo ayudarte con algo más?", message);
                        return;
                    }
                      // Si confirma, procesar la reserva
                    // Crear la reserva con estado "Solicitud" en lugar de "Pendiente"
                    var nuevaReserva = new Reserva
                    {
                        Cliente = reserva.Cliente,
                        Telefono = reserva.Telefono,
                        CantidadPersonas = reserva.CantidadPersonas,
                        Fecha = reserva.Fecha,
                        Hora = reserva.Hora,
                        Estado = "Solicitud", // Cambiado de "Pendiente" a "Solicitud"
                        Observaciones = reserva.Observaciones,
                        ClienteId = message.From.Id,
                        MontoSeguro = MONTO_SEGURO,
                        SeguroPagado = false,
                        FechaCreacion = DateTime.Now
                    };

                    // Guardar la reserva en la base de datos
                    new ReservaRepository().AgregarReserva(nuevaReserva);

                    // Disparar evento de nueva reserva para notificaciones
                    EnhancedTelegramService.Instance.OnReservaCreada(nuevaReserva);// Crear teclado con acciones posteriores
                    var tecladoFinal = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[] 
                        { 
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🛍️ Hacer un pedido", "pedir"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Volver al inicio", "start") 
                        }
                    });                    // Mensaje actualizado para informar sobre el sistema de seguro
                    await Responder($"📌 ¡Solicitud de reserva enviada!\n\n" +
                                  $"Tu reserva será revisada por el restaurante. Si es aceptada, se requerirá un seguro de ${MONTO_SEGURO:N0} COP que será:\n"+
                                  $"✅ Reembolsado o descontado de tu consumo si asistes\n" +
                                  $"❌ Retenido por el restaurante si no te presentas\n\n" +
                                  $"Te notificaremos cuando tengamos una respuesta.", message, tecladoFinal);                    
                    _reservasEnProceso.Remove(clientId);
                    break;
            }
        }        private async Task Responder(string texto, TelegramMessage mensaje, Telegram.Bot.Types.ReplyMarkups.IReplyMarkup? replyMarkup = null)
        {
            if (mensaje.Chat == null || mensaje.From == null)
                return;
            
            long clientId = mensaje.From.Id;
            
            // Verificar si existe una reserva en proceso y obtener el paso actual
            PasoReserva? pasoActual = null;
            if (_reservasEnProceso.ContainsKey(clientId))
            {
                pasoActual = _reservasEnProceso[clientId].PasoActual;
            }
            
            // Si el paso actual es Confirmacion o posterior, enviar siempre mensaje nuevo
            bool debeEnviarNuevo = pasoActual.HasValue && 
                                   (pasoActual.Value >= PasoReserva.Confirmacion);
            
            // Si el mensaje tiene MessageId y viene de un callback, y NO estamos en paso de confirmación o posterior, intentar editar
            if (mensaje.MessageId != 0 && mensaje.Chat != null && !debeEnviarNuevo)
            {
                try
                {
                    await _bot.EditMessageTextAsync(
                        chatId: mensaje.Chat.Id,
                        messageId: mensaje.MessageId,
                        text: texto,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: replyMarkup as Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup
                    );
                }
                catch
                {
                    // Si falla la edición (por ejemplo, mensaje antiguo), enviar uno nuevo
                    await _bot.SendTextMessageAsync(
                        chatId: mensaje.Chat.Id,
                        text: texto,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: replyMarkup
                    );
                }
            }
            else
            {
                // Enviar mensaje nuevo (caso normal o cuando estamos en Confirmacion o posterior)
                await _bot.SendTextMessageAsync(
                    chatId: mensaje.Chat.Id,
                    text: texto,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: replyMarkup
                );
            }

            _onMessage?.Invoke(new AppMessage
            {
                Text = texto,
                Timestamp = DateTime.Now,
                IsFromAdmin = true,
                ClientId = mensaje.From.Id
            });
        }
    }    public enum PasoReserva
    {
        Cantidad,
        CantidadPersonalizada,
        Fecha,
        Hora,
        Observaciones,
        Confirmacion
    }

    public class ReservaEnProceso
    {
        public string Cliente { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public int CantidadPersonas { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public PasoReserva PasoActual { get; set; }
    }
}
