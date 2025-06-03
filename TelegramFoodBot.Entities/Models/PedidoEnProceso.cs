using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramFoodBot.Entities.Models
{
    public class PedidoEnProceso
    {
        public long ClienteId { get; set; }
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Productos { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = string.Empty;
        public int? CuentaPagoSeleccionadaId { get; set; } // Nueva propiedad para almacenar la cuenta seleccionada

        // Puedes agregar esta propiedad para saber en qué paso va
        public PasoPedido PasoActual { get; set; } = PasoPedido.Direccion;
    }

    public enum PasoPedido
    {
        Direccion,
        Productos,
        Observaciones,
        MetodoPago,
        SeleccionCuentaPago, // Nuevo paso para seleccionar cuenta específica
        CuentasPago,
        Confirmado
    }

    public class PedidoConCliente
    {
        public string Id { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Productos { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }

}
