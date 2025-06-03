using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFoodBot.Business.Interfaces;
using TelegramFoodBot.Business.Configuration;

namespace TelegramFoodBot.Business.Commands.Handlers
{
    /// <summary>
    /// Handler para callbacks de métodos de pago
    /// Aplica el principio de Responsabilidad Única (SRP)
    /// Implementa el patrón Strategy para manejo de callbacks específicos de pago
    /// </summary>
    public class PagoCallbackHandler : ICallbackHandler
    {
        private readonly ComandoPedido _comandoPedido;

        public PagoCallbackHandler(ComandoPedido comandoPedido)
        {
            _comandoPedido = comandoPedido;
        }

        /// <summary>
        /// Determina si este handler puede manejar el callback dado
        /// </summary>
        public bool CanHandle(string callbackData)
        {
            return callbackData == "pago_efectivo" || callbackData == "pago_transferencia";
        }

        /// <summary>
        /// Maneja los callbacks de métodos de pago
        /// Simula una respuesta del usuario para mantener la compatibilidad con el flujo existente
        /// </summary>
        public async Task HandleCallback(CallbackQuery callbackQuery, ITelegramService telegramService)
        {
            var clientId = callbackQuery.From.Id;
            
            // Verificar que el usuario esté en proceso de pedido
            if (!_comandoPedido.EnPedido(clientId))
            {
                var errorMessage = CreateFakeMessage(callbackQuery, "");
                if (errorMessage != null)
                    await RespondError(errorMessage, "❌ No tienes un pedido en proceso. Usa /pedir para comenzar uno nuevo.");
                return;
            }

            // Convertir el callback a texto legible para el usuario
            string metodoSeleccionado = callbackQuery.Data switch
            {
                "pago_efectivo" => "Efectivo",
                "pago_transferencia" => "Transferencia",
                _ => "Método no válido"
            };

            // Simular respuesta del usuario para reutilizar la lógica existente del ComandoPedido
            var fakeMessage = CreateFakeMessage(callbackQuery, metodoSeleccionado);
            
            if (fakeMessage != null)
                await _comandoPedido.Ejecutar(fakeMessage);
        }

        /// <summary>
        /// Crea un mensaje simulado basado en el callback para mantener compatibilidad
        /// con el flujo existente del ComandoPedido
        /// </summary>
        private Message? CreateFakeMessage(CallbackQuery callbackQuery, string text)
        {
            if (callbackQuery.Message?.Chat == null || callbackQuery.From == null)
                return null;
                
            return new Message
            {
                Chat = callbackQuery.Message.Chat,
                From = callbackQuery.From,
                MessageId = callbackQuery.Message.MessageId,
                Date = System.DateTime.UtcNow,
                Text = text
            };
        }

        /// <summary>
        /// Envía un mensaje de error al usuario
        /// </summary>
        private async Task RespondError(Message message, string errorText)
        {            // En una implementación ideal, el bot client sería inyectado como dependencia
            // Por consistencia con el código existente, usamos la misma aproximación
            var botClient = new Telegram.Bot.TelegramBotClient(BotConfiguration.BotToken);
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: errorText
            );
        }
    }
}
