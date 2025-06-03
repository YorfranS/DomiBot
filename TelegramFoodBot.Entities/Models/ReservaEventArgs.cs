using System;

namespace TelegramFoodBot.Entities.Models
{
    public class ReservaEventArgs : EventArgs
    {
        public Reserva Reserva { get; }
        public ReservaEventArgs(Reserva reserva)
        {
            Reserva = reserva;
        }
    }
}
