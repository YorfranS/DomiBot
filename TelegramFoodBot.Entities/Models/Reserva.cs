using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramFoodBot.Entities.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public string Cliente { get; set; }
        public string Telefono { get; set; }
        public int CantidadPersonas { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public string Estado { get; set; } // Solicitud, Aceptada, Confirmada, Cancelada, Rechazada, Completada, NoShow
        public string Observaciones { get; set; }
        public long ClienteId { get; set; }
        public decimal MontoSeguro { get; set; }
        public bool SeguroPagado { get; set; }
        public DateTime? FechaPagoSeguro { get; set; }
        public string MetodoPagoSeguro { get; set; }
        public bool ClienteAsistio { get; set; }
        public DateTime? FechaAsistencia { get; set; }
        public bool SeguroReembolsado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}

