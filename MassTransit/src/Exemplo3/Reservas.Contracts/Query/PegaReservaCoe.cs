using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reservas.Contracts.Commands
{
    public class PegaReservaCoe
    {
        public PegaReservaCoe(Guid idReserva)
        {
            IdReserva = idReserva;
        }

        public Guid IdReserva { get; }
    }

    public class PegaReservaCoeResp
    {
        [JsonConstructor]
        public PegaReservaCoeResp(Guid idReserva, int cliente, decimal valorReservado, string codigoCOE, string estado, DateTime dataReserva, bool reservaEncontrada = true)
        {
            IdReserva = idReserva;
            Cliente = cliente;
            ValorReservado = valorReservado;
            CodigoCOE = codigoCOE;
            Estado = estado;
            DataReserva = dataReserva;
            ReservaEncontrada = reservaEncontrada;
        }

        private PegaReservaCoeResp()
        {
            ReservaEncontrada = false;
        }


        public static PegaReservaCoeResp NaoEncontrada => new PegaReservaCoeResp();


        public bool ReservaEncontrada { get; }

        public Guid IdReserva { get; }
        public int Cliente { get; }
        public decimal ValorReservado { get; }
        public string CodigoCOE { get; }
        public string Estado { get; }
        public DateTime DataReserva { get; }

    }


    public class ReservaNaoEcontrada
    {

    }
}
