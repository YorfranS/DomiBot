using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFoodBot.Business.Interfaces;

namespace TelegramFoodBot.Business.Services
{
    /// <summary>
    /// Servicio que maneja callbacks de botones siguiendo el patrón Strategy y Command
    /// Implementa los principios SOLID: Single Responsibility y Open/Closed
    /// </summary>
    public class CallbackHandlerService
    {
        private readonly List<ICallbackHandler> _handlers;
        private readonly TelegramBotClient _botClient;

        public CallbackHandlerService(TelegramBotClient botClient)
        {
            _botClient = botClient;
            _handlers = new List<ICallbackHandler>();
        }

        /// <summary>
        /// Registra un nuevo handler (Open/Closed Principle)
        /// </summary>
        public void RegisterHandler(ICallbackHandler handler)
        {
            _handlers.Add(handler);
        }

        /// <summary>
        /// Procesa un callback utilizando el handler apropiado
        /// </summary>
        public async Task ProcessCallback(CallbackQuery callbackQuery, ITelegramService telegramService)
        {
            try
            {
                if (string.IsNullOrEmpty(callbackQuery.Data))
                {
                    Console.WriteLine("Callback recibido sin data");
                    return;
                }
                
                var handler = _handlers.FirstOrDefault(h => h.CanHandle(callbackQuery.Data));
                
                if (handler != null)
                {
                    await handler.HandleCallback(callbackQuery, telegramService);
                }
                else
                {
                    // Log del callback no manejado
                    Console.WriteLine($"Callback no manejado: {callbackQuery.Data}");
                }

                // Siempre responder al callback para quitar el spinner
                await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesando callback {callbackQuery.Data}: {ex.Message}");
                
                // Notificar al usuario del error
                try
                {
                    await _botClient.AnswerCallbackQueryAsync(
                        callbackQuery.Id, 
                        "❌ Hubo un error procesando tu solicitud. Intenta de nuevo.", 
                        showAlert: true
                    );
                }
                catch
                {
                    // Si falla la respuesta de error, al menos responder el callback
                    try
                    {
                        await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    }
                    catch { /* Última línea de defensa */ }
                }
            }
        }
    }
}
