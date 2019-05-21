using System;
using System.Collections.Generic;
using System.Text;

namespace Reservas.Contracts.Events
{
    public class ReservaRejeitada
    {
        public ReservaRejeitada(string motivo)
        {
            Motivo = motivo;
        }

        public string Motivo { get; }

    }
}
