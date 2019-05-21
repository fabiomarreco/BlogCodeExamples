using System;
using System.Collections.Generic;
using System.Text;

namespace Reservas.Contracts.Events
{
    public class ReservaCancelada
    {
        public ReservaCancelada(Guid idReserva)
        {
            IdReserva = idReserva;
        }

        public Guid IdReserva { get; }
    }
}
