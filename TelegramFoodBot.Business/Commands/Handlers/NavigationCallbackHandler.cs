using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFoodBot.Business.Interfaces;

namespace TelegramFoodBot.Business.Commands.Handlers
{
    /// <summary>
    /// Handler para callbacks de navegación (start, menu principal)
    /// Implementa el principio de Responsabilidad Única
    /// </summary>
    public class NavigationCallbackHandler : ICallbackHandler
    {
        private readonly TelegramBotClient _botClient;

        public NavigationCallbackHandler(TelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public bool CanHandle(string callbackData)
        {
            return callbackData == "start" || callbackData == "menu_principal";
        }        public async Task HandleCallback(CallbackQuery callbackQuery, ITelegramService telegramService)
        {
            if (callbackQuery.Message?.Chat == null) return;
            
            switch (callbackQuery.Data)
            {
                case "start":
                case "menu_principal":
                    await ShowMainMenu(callbackQuery);
                    break;
            }
        }

        private async Task ShowMainMenu(CallbackQuery callbackQuery)
        {
            if (callbackQuery.Message?.Chat == null) return;
            
            // Crear teclado inline con botones principales
            var tecladoInline = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Registrarse", "registrarse"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🛍️ Hacer Pedido", "pedir")
                },
                new[]
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📅 Reservar Mesa", "reservar")
                }
            });

            const string mensajeBienvenida = "🎉 ¡Bienvenido a DomiBot! 🍔📱\n" +
                "Soy tu asistente para pedir comida y reservar tu mesa en segundos 😄.\n" +
                "¿Qué quieres hacer hoy?\n\n" +
                "👇 *Toca uno de los botones* para comenzar, o escribe uno de los siguientes comandos:\n\n" +
                "✅ /registrarse – ¡Activa tu cuenta y empieza a disfrutar!\n" +
                "🛍️ /pedir – Haz tu pedido en pocos pasos, ¡así de fácil!\n" +
                "📅 /reservar – Reserva tu mesa con seguro incluido 🛡️";

            // Editar el mensaje existente en lugar de enviar uno nuevo
            try
            {
                await _botClient.EditMessageTextAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    messageId: callbackQuery.Message.MessageId,
                    text: mensajeBienvenida,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: tecladoInline
                );
            }
            catch
            {
                // Si falla la edición, enviar mensaje nuevo
                await _botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: mensajeBienvenida,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: tecladoInline
                );
            }
        }
    }
}