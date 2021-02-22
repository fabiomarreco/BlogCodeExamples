using System;
using System.Collections.Generic;
using System.Text;

namespace Reservas.Contracts.Commands
{
    public class ConcluiReservaCoe
    {
        public ConcluiReservaCoe(string codigo)
        {
            Codigo = codigo;
        }

        public string Codigo { get; }
    }
}
