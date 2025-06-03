using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramFoodBot.Entities.Models
{    public class Pedido
    {
        public string Id { get; set; }
        public long ClienteId { get; set; }
        public string TelefonoEntrega { get; set; }
        public string DireccionEntrega { get; set; }
        public string Productos { get; set; }
        public string Observaciones { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; }
        public DateTime FechaHora { get; set; }
        
        // Campos para confirmación de pago
        public bool PagoPendiente { get; set; }
        public bool PagoConfirmado { get; set; }
        public DateTime? FechaConfirmacionPago { get; set; }
        public string MetodoConfirmacionPago { get; set; }
    }

    public class DetallePedido
    {
        public int Id { get; set; }  // si usas identity, no lo pones en insert
        public string PedidoId { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
    }
}

