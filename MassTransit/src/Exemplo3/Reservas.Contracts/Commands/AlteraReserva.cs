using System;
using System.Collections.Generic;
using System.Text;

namespace Reservas.Contracts.Commands
{
    public class AlteraReserva
    {
        public AlteraReserva(int cliente, string codigoCoe, decimal novoValor)
        {
            Cliente = cliente;
            CodigoCoe = codigoCoe;
            NovoValor = novoValor;
        }

        public int Cliente { get;  }
        public string CodigoCoe { get; }
        public decimal NovoValor { get; }
    }
}
