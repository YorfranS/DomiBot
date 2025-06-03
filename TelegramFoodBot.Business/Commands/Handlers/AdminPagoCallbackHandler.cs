using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFoodBot.Business.Interfaces;
using TelegramFoodBot.Data;

namespace TelegramFoodBot.Business.Commands.Handlers
{
    /// <summary>
    /// Handler para callbacks de confirmación de pago por parte del administrador
    /// Maneja las acciones de confirmar/rechazar pagos desde Telegram
    /// </summary>
    public class AdminPagoCallbackHandler : ICallbackHandler
    {
        private readonly PedidoRepository _pedidoRepo;

        public AdminPagoCallbackHandler()
        {
            _pedidoRepo = new PedidoRepository();
        }

        /// <summary>
        /// Determina si este handler puede manejar el callback dado
        /// </summary>
        public bool CanHandle(string callbackData)
        {
            return callbackData.StartsWith("confirmar_pago_") || callbackData.StartsWith("rechazar_pago_");
        }

        /// <summary>
        /// Maneja los callbacks de confirmación/rechazo de pago por parte del admin
        /// </summary>
        public async Task HandleCallback(CallbackQuery callbackQuery, ITelegramService telegramService)
        {
            var callbackData = callbackQuery.Data;
            var adminId = callbackQuery.From.Id;
              // Verificar que sea un administrador (esto debería mejorarse con un sistema de roles)
            if (!EsAdministrador(adminId))
            {
                telegramService.SendMessage(adminId, "❌ No tienes permisos para realizar esta acción.");
                return;
            }            if (callbackData.StartsWith("confirmar_pago_"))
            {
                var pedidoId = callbackData.Replace("confirmar_pago_", "");
                await ConfirmarPagoPedido(pedidoId, telegramService, callbackQuery.Message?.Chat.Id ?? 0);
            }
            else if (callbackData.StartsWith("rechazar_pago_"))
            {
                var pedidoId = callbackData.Replace("rechazar_pago_", "");
                await RechazarPagoPedido(pedidoId, telegramService, callbackQuery.Message?.Chat.Id ?? 0);
            }
        }        private async Task ConfirmarPagoPedido(string pedidoId, ITelegramService telegramService, long chatId)
        {
            try
            {
                // Obtener el pedido para verificar estado
                var pedidos = _pedidoRepo.ObtenerPedidosPendientesPago();
                var pedido = pedidos.Find(p => p.Id == pedidoId);
                
                if (pedido == null)
                {
                    telegramService.SendMessage(chatId, $"❌ No se encontró el pedido #{pedidoId} o ya no está pendiente de pago.");
                    return;
                }

                // Confirmar el pago
                _pedidoRepo.ConfirmarPagoPedido(pedidoId, "Transferencia");
                
                // Notificar al cliente
                string mensajeCliente = $"✅ ¡Excelente! Hemos confirmado tu pago para el pedido #{pedidoId}. Tu pedido ahora está en preparación. ¡Te avisaremos cuando esté listo!";
                telegramService.SendMessage(pedido.ClienteId, mensajeCliente);
                
                // Confirmar al admin
                telegramService.SendMessage(chatId, $"✅ Pago confirmado para el pedido #{pedidoId}. Cliente notificado.");
            }
            catch (System.Exception ex)
            {
                telegramService.SendMessage(chatId, $"❌ Error al confirmar el pago: {ex.Message}");
            }
        }        private async Task RechazarPagoPedido(string pedidoId, ITelegramService telegramService, long chatId)
        {
            try
            {
                // Obtener el pedido para verificar estado
                var pedidos = _pedidoRepo.ObtenerPedidosPendientesPago();
                var pedido = pedidos.Find(p => p.Id == pedidoId);
                
                if (pedido == null)
                {
                    telegramService.SendMessage(chatId, $"❌ No se encontró el pedido #{pedidoId} o ya no está pendiente de pago.");
                    return;
                }

                // Rechazar el pago
                _pedidoRepo.RechazarPagoPedido(pedidoId);
                
                // Notificar al cliente
                string mensajeCliente = $"❌ Lo sentimos, no pudimos confirmar el pago para tu pedido #{pedidoId}. El pedido ha sido cancelado. Si realizaste la transferencia, por favor contactanos para resolver el inconveniente.";
                telegramService.SendMessage(pedido.ClienteId, mensajeCliente);
                
                // Confirmar al admin
                telegramService.SendMessage(chatId, $"❌ Pago rechazado para el pedido #{pedidoId}. Pedido cancelado y cliente notificado.");
            }
            catch (System.Exception ex)
            {
                telegramService.SendMessage(chatId, $"❌ Error al rechazar el pago: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica si el usuario es administrador
        /// TODO: Implementar un sistema de roles más robusto
        /// </summary>
        private bool EsAdministrador(long userId)
        {
            // Por ahora, hardcodeamos IDs de admin - esto debería mejorarse
            var adminIds = new long[] { 1066516207, 123456789 }; // Agregar IDs reales de admin aquí
            return System.Array.Exists(adminIds, id => id == userId);
        }
    }
}
