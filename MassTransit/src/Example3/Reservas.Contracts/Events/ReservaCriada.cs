using System;
using System.Collections.Generic;
using System.Text;

namespace Reservas.Contracts.Events
{
    public class ReservaCriada
    {
        public ReservaCriada(Guid idReserva, int cliente, string codigoCOE, DateTime data, decimal valor)
        {
            IdReserva = idReserva;
            Cliente = cliente;
            CodigoCOE = codigoCOE;
            Data = data;
            Valor = valor;
        }


        public Guid IdReserva { get; set;  }

        public int Cliente { get; }

        public string CodigoCOE { get; set; }

        public DateTime Data { get; }

        public decimal Valor { get; }
    }
}
