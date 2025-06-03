using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFoodBot.Business.Interfaces;
using TelegramFoodBot.Business.Configuration;

namespace TelegramFoodBot.Business.Commands.Handlers
{
    /// <summary>
    /// Handler para callbacks de confirmación de pedidos
    /// Aplica el principio de Responsabilidad Única
    /// </summary>
    public class PedidoCallbackHandler : ICallbackHandler
    {
        private readonly ComandoPedido _comandoPedido;

        public PedidoCallbackHandler(ComandoPedido comandoPedido)
        {
            _comandoPedido = comandoPedido;
        }        public bool CanHandle(string callbackData)
        {
            return callbackData == "pedido_confirmar" || callbackData == "pedido_cancelar" || callbackData == "continuar_pedido";
        }        public async Task HandleCallback(CallbackQuery callbackQuery, ITelegramService telegramService)
        {
            var clientId = callbackQuery.From.Id;
            var callbackData = callbackQuery.Data;
            
            // Log para diagnóstico
            Console.WriteLine($"[PEDIDO_CALLBACK] Procesando: {callbackData} para usuario {clientId}");
            
            // Verificar que el usuario esté en proceso de pedido
            if (!_comandoPedido.EnPedido(clientId))
            {
                Console.WriteLine($"[PEDIDO_ERROR] Usuario {clientId} no tiene un pedido en proceso para callback {callbackData}");
                
                // Crear mensaje de error si no está en proceso
                var errorMessage = CreateFakeMessage(callbackQuery, "");
                if (errorMessage != null)
                    await RespondError(errorMessage, "No tienes un pedido en proceso. Usa /pedir para comenzar uno nuevo.");
                return;
            }
            
            // Simular respuesta del usuario para reutilizar la lógica existente
            string simulatedResponse = callbackQuery.Data switch
            {
                "pedido_confirmar" => "Sí",
                "pedido_cancelar" => "No",
                "continuar_pedido" => "continuar",
                _ => ""
            };
            
            Console.WriteLine($"[PEDIDO_CALLBACK] Simulando respuesta: '{simulatedResponse}' para callback {callbackData}");
            
            var fakeMessage = CreateFakeMessage(callbackQuery, simulatedResponse);
            
            if (fakeMessage != null)
            {
                try 
                {
                    await _comandoPedido.Ejecutar(fakeMessage);
                    Console.WriteLine($"[PEDIDO_SUCCESS] Callback {callbackData} procesado para usuario {clientId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[PEDIDO_ERROR] Error al procesar callback {callbackData}: {ex.Message}");
                    throw; // Re-lanzar para manejo superior
                }
            }
        }

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

        private async Task RespondError(Message message, string errorText)        {
            // Método auxiliar para enviar mensajes de error
            // En una implementación real, esto podría ser inyectado como dependencia
            var botClient = new Telegram.Bot.TelegramBotClient(BotConfiguration.BotToken);
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: errorText
            );
        }
    }
}