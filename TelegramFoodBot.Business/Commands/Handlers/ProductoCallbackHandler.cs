using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramFoodBot.Business.Services;
using TelegramFoodBot.Business.Interfaces;
using TelegramMessage = Telegram.Bot.Types.Message;

namespace TelegramFoodBot.Business.Commands.Handlers
{
    /// <summary>
    /// Handler para callbacks de selección de productos
    /// Maneja las acciones relacionadas con la selección de productos del menú
    /// </summary>
    public class ProductoCallbackHandler : ICallbackHandler
    {
        private readonly ComandoPedido _comandoPedido;

        public ProductoCallbackHandler(ComandoPedido comandoPedido)
        {
            _comandoPedido = comandoPedido;
        }        public bool CanHandle(string callbackData)
        {
            return callbackData.StartsWith("categoria_") || 
                   callbackData.StartsWith("producto_") || 
                   callbackData.StartsWith("promocion_producto_") ||
                   callbackData.StartsWith("cuenta_pago_") ||
                   IsProductQuantityCallback(callbackData) ||
                   callbackData == "volver_menu" ||
                   callbackData == "finalizar_productos" ||
                   callbackData == "observaciones_si" ||
                   callbackData == "observaciones_no" ||
                   callbackData == "volver_metodo_pago" ||
                   callbackData == "volver_cuentas_pago";
        }

        /// <summary>
        /// Verifica si el callback es de cantidad para productos (formato: cantidad_productoId_cantidad)
        /// </summary>
        /// <param name="callbackData">Datos del callback</param>
        /// <returns>True si es un callback de cantidad para productos</returns>
        private bool IsProductQuantityCallback(string callbackData)
        {
            if (!callbackData.StartsWith("cantidad_"))
                return false;
                
            var partes = callbackData.Split('_');
            // Debe tener exactamente 3 partes: "cantidad", "productoId", "cantidad"
            // y las partes 2 y 3 deben ser números válidos
            return partes.Length == 3 &&
                   int.TryParse(partes[1], out _) &&
                   int.TryParse(partes[2], out _);
        }

        public async Task HandleCallback(CallbackQuery callbackQuery, ITelegramService telegramService)
        {
            // Pasar el mensaje original con MessageId para permitir edición interactiva
            if (callbackQuery.Message != null && callbackQuery.From != null)
            {
                var message = new Telegram.Bot.Types.Message
                {
                    MessageId = callbackQuery.Message.MessageId,
                    From = callbackQuery.From,
                    Chat = callbackQuery.Message.Chat,
                    Text = callbackQuery.Data,
                    Date = DateTime.UtcNow
                };
                await _comandoPedido.EjecutarCallbackProducto(message);
            }
        }

        private Message? CreateFakeMessage(CallbackQuery callbackQuery, string text)
        {
            if (callbackQuery.From == null) return null;

            return new Message
            {
                MessageId = callbackQuery.Message?.MessageId ?? 0,
                From = callbackQuery.From,
                Chat = callbackQuery.Message?.Chat ?? new Chat 
                { 
                    Id = callbackQuery.From.Id, 
                    Type = Telegram.Bot.Types.Enums.ChatType.Private 
                },
                Text = text,
                Date = DateTime.UtcNow
            };
        }
    }
}
