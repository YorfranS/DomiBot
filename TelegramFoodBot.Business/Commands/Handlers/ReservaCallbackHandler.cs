using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFoodBot.Business.Interfaces;
using TelegramFoodBot.Business.Configuration;

namespace TelegramFoodBot.Business.Commands.Handlers
{
    /// <summary>
    /// Handler para callbacks de confirmación de reservas
    /// Implementa el principio de Responsabilidad Única
    /// </summary>
    public class ReservaCallbackHandler : ICallbackHandler
    {
        private readonly ComandoReserva _comandoReserva;

        public ReservaCallbackHandler(ComandoReserva comandoReserva)
        {
            _comandoReserva = comandoReserva;
        }        public bool CanHandle(string callbackData)
        {
            return callbackData == "reserva_confirmar" || 
                   callbackData == "reserva_cancelar" || 
                   callbackData == "observacion_si" || 
                   callbackData == "observacion_no" ||
                   callbackData == "fecha_hoy" ||
                   callbackData == "fecha_manana" ||
                   callbackData == "fecha_otra" ||
                   callbackData.StartsWith("cantidad_");
        }        public async Task HandleCallback(CallbackQuery callbackQuery, ITelegramService telegramService)
        {
            var clientId = callbackQuery.From.Id;
            var callbackData = callbackQuery.Data;
            
            // Log para diagnóstico
            Console.WriteLine($"[RESERVA_CALLBACK] Procesando: {callbackData} para usuario {clientId}");

            // Verificar que el usuario esté en proceso de reserva
            if (!_comandoReserva.EnReserva(clientId))
            {
                Console.WriteLine($"[RESERVA_ERROR] Usuario {clientId} no tiene una reserva en proceso para callback {callbackData}");
                
                // Crear mensaje de error si no está en proceso
                var errorMessage = CreateFakeMessage(callbackQuery, "");
                if (errorMessage != null)
                    await RespondError(errorMessage, "No tienes una reserva en proceso. Usa /reservar para comenzar una nueva.");
                return;
            }
            
            // Simular respuesta del usuario para reutilizar la lógica existente
            string simulatedResponse = callbackQuery.Data switch
            {
                "reserva_confirmar" => "Sí",
                "reserva_cancelar" => "No",
                "observacion_si" => "observacion_si",
                "observacion_no" => "observacion_no",
                "fecha_hoy" => "fecha_hoy",
                "fecha_manana" => "fecha_manana",
                "fecha_otra" => "fecha_otra",
                var data when data != null && data.StartsWith("cantidad_") => data,
                _ => ""
            };
            
            Console.WriteLine($"[RESERVA_CALLBACK] Simulando respuesta: '{simulatedResponse}' para callback {callbackData}");
            
            var fakeMessage = CreateFakeMessage(callbackQuery, simulatedResponse);

            if (fakeMessage != null)
            {
                try
                {
                    await _comandoReserva.Ejecutar(fakeMessage);
                    Console.WriteLine($"[RESERVA_SUCCESS] Callback {callbackData} procesado para usuario {clientId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RESERVA_ERROR] Error al procesar callback {callbackData}: {ex.Message}");
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
            var botClient = new Telegram.Bot.TelegramBotClient(BotConfiguration.BotToken);
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: errorText
            );
        }
    }
}