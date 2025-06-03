using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFoodBot.Business.Interfaces;

namespace TelegramFoodBot.Business.Commands.Handlers
{
    /// <summary>
    /// Handler para callbacks de comandos principales (Registrarse, Pedir, Reservar)
    /// Implementa Single Responsibility Principle
    /// </summary>
    public class MainCommandCallbackHandler : ICallbackHandler
    {
        private readonly ComandoRegistro _comandoRegistro;
        private readonly ComandoPedido _comandoPedido;
        private readonly ComandoReserva _comandoReserva;

        public MainCommandCallbackHandler(
            ComandoRegistro comandoRegistro,
            ComandoPedido comandoPedido,
            ComandoReserva comandoReserva)
        {
            _comandoRegistro = comandoRegistro;
            _comandoPedido = comandoPedido;
            _comandoReserva = comandoReserva;
        }        public bool CanHandle(string callbackData)
        {
            return callbackData == "registrarse" || 
                   callbackData == "pedir" || 
                   callbackData == "reservar";
        }public async Task HandleCallback(CallbackQuery callbackQuery, ITelegramService telegramService)
        {
            // Crear un mensaje simulado para mantener compatibilidad con comandos existentes
            var fakeMessage = CreateFakeMessage(callbackQuery, "/" + callbackQuery.Data);
            if (fakeMessage == null) return;

            switch (callbackQuery.Data)
            {
                case "registrarse":
                    await _comandoRegistro.Ejecutar(fakeMessage);
                    break;
                case "pedir":
                    await _comandoPedido.Ejecutar(fakeMessage);
                    break;
                case "reservar":
                    await _comandoReserva.Ejecutar(fakeMessage);
                    break;
            }
        }private Message? CreateFakeMessage(CallbackQuery callbackQuery, string text)
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
    }
}