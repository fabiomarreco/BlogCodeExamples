using System;

namespace Reservas.Contracts
{
    public class CriaCOE
    {
        public CriaCOE(string descricao, DateTime vencimento, DateTime dataFimReserva)
        {
            Descricao = descricao;
            Vencimento = vencimento;
            DataFimReserva = dataFimReserva;
        }

        public string Descricao { get; }
        public DateTime Vencimento { get; }
        public DateTime DataFimReserva { get; }
    }
}
