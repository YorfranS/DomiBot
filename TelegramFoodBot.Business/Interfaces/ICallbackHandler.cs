using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramFoodBot.Business.Interfaces
{
    /// <summary>
    /// Interfaz para manejar callbacks de botones inline siguiendo el principio de Responsabilidad Única
    /// </summary>
    public interface ICallbackHandler
    {
        /// <summary>
        /// Verifica si este handler puede manejar el callback específico
        /// </summary>
        bool CanHandle(string callbackData);
        
        /// <summary>
        /// Procesa el callback del botón
        /// </summary>
        Task HandleCallback(CallbackQuery callbackQuery, ITelegramService telegramService);
    }
}