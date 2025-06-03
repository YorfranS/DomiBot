using System;

namespace TelegramFoodBot.Entities.Models
{
    public class PedidoEventArgs : EventArgs
    {
        public Pedido Pedido { get; }
        public PedidoEventArgs(Pedido pedido)
        {
            Pedido = pedido;
        }
    }
}
