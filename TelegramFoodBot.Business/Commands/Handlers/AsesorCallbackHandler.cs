using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFoodBot.Business.Interfaces;
using TelegramFoodBot.Business.Configuration;

namespace TelegramFoodBot.Business.Commands.Handlers
{
    /// <summary>
    /// Handler para callbacks de contacto con asesor
    /// Maneja las solicitudes de contacto con asesores cuando hay problemas con pagos
    /// </summary>
    public class AsesorCallbackHandler : ICallbackHandler
    {
        /// <summary>
        /// Determina si este handler puede manejar el callback dado
        /// </summary>
        public bool CanHandle(string callbackData)
        {
            return callbackData == "hablar_asesor";
        }

        /// <summary>
        /// Maneja el callback de contacto con asesor
        /// Proporciona información de contacto al usuario
        /// </summary>
        public async Task HandleCallback(CallbackQuery callbackQuery, ITelegramService telegramService)
        {
            var clientId = callbackQuery.From.Id;
            
            // Mensaje con información de contacto del asesor
            string mensajeAsesor = "👨‍💼 **CONTACTO CON ASESOR**\n\n" +
                                 "Entendemos que puedas tener dudas sobre tu pago. Nuestro equipo está aquí para ayudarte:\n\n" +
                                 "📱 **WhatsApp:** +57 300 123 4567\n" +
                                 "📞 **Teléfono:** (1) 234-5678\n" +
                                 "📧 **Email:** soporte@domibot.com\n" +
                                 "⏰ **Horario:** Lunes a Domingo 8:00 AM - 10:00 PM\n\n" +
                                 "💡 **¿Qué necesitas hacer?**\n" +
                                 "• Envíanos el comprobante de transferencia\n" +
                                 "• Indica tu número de pedido\n" +
                                 "• Describe tu consulta\n\n" +
                                 "🚀 **Respuesta rápida:** También puedes continuar con tu pedido y nuestros asesores verificarán tu pago automáticamente.";

            // Crear botones adicionales para el usuario
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithUrl("📱 WhatsApp", "https://wa.me/573001234567"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Continuar Pedido", "continuar_pedido")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Menú Principal", "menu_principal")
                }
            });

            // Enviar mensaje al cliente
            await SendMessageSafely(telegramService, clientId, mensajeAsesor, keyboard);
            
            // Notificar a los administradores sobre la solicitud de contacto
            await NotificarSolicitudAsesor(telegramService, clientId, callbackQuery.From.FirstName ?? "Usuario");
        }

        /// <summary>
        /// Envía mensaje de manera segura manejando posibles errores
        /// </summary>
        private async Task SendMessageSafely(ITelegramService telegramService, long chatId, string message, Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup keyboard = null)
        {
            try
            {
                if (keyboard != null)                {
                    // Usar el método estándar del servicio (asumiendo que tiene soporte para keyboard)
                    var botClient = new Telegram.Bot.TelegramBotClient(BotConfiguration.BotToken);
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: message,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: keyboard
                    );
                }
                else
                {
                    telegramService.SendMessage(chatId, message);
                }
            }
            catch (System.Exception ex)
            {
                // Log del error y envío de mensaje básico como fallback
                System.Console.WriteLine($"Error enviando mensaje con botones: {ex.Message}");
                telegramService.SendMessage(chatId, message);
            }
        }

        /// <summary>
        /// Notifica a los administradores sobre la solicitud de contacto con asesor
        /// </summary>
        private async Task NotificarSolicitudAsesor(ITelegramService telegramService, long clientId, string nombreCliente)
        {
            var adminIds = new long[] { 1066516207 }; // IDs de administradores
            
            string mensajeAdmin = $"🔔 **SOLICITUD DE ASESOR**\n\n" +
                                $"👤 Cliente: {nombreCliente} (ID: {clientId})\n" +
                                $"🕒 Hora: {System.DateTime.Now:dd/MM/yyyy HH:mm}\n" +
                                $"❓ Motivo: Consulta sobre pago por transferencia\n\n" +
                                $"El cliente necesita asistencia con su proceso de pago. " +
                                $"Por favor, contactar lo antes posible.";

            foreach (var adminId in adminIds)
            {
                try
                {
                    telegramService.SendMessage(adminId, mensajeAdmin);
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"Error notificando admin {adminId}: {ex.Message}");
                }
            }
        }
    }
}
