using System;
using System.Collections.Generic;
using System.Text;

namespace Reservas.Contracts.Commands
{
    public class CancelaReserva
    {
        public CancelaReserva(Guid idReserva)
        {
            IdReserva = idReserva;
        }

        public Guid IdReserva { get; }
    }
}
