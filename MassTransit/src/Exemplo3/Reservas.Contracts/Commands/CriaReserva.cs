using System;
using System.Collections.Generic;
using System.Text;

namespace Reservas.Contracts.Commands
{
    public class CriaReserva
    {
        public CriaReserva(int cliente, string codigoCOE, decimal valor)
        {
            Cliente = cliente;
            CodigoCOE = codigoCOE;
            Valor = valor;
        }

        public int Cliente { get; }

        public string CodigoCOE { get; set; }

        public decimal Valor { get; }
    }
}
