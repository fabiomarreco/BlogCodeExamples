using System;
using System.Collections.Generic;
using System.Text;

namespace Reservas.Contracts.Events
{
    public class PeriodoReservaCOEConcluido
    {
        public PeriodoReservaCOEConcluido(string codigoCOE)
        {
            CodigoCOE = codigoCOE;
        }

        public string CodigoCOE { get; }
    }
}
