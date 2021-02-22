using System;
using System.Collections.Generic;
using System.Text;

namespace ContaCorrente.Contracts.Commands
{
    public class GeraLancamento
    {
        public GeraLancamento(int account, decimal valor)
        {
            Account = account;
            Valor = valor;
        }

        public int Account { get; }

        public decimal Valor { get; }
    }


    public class GeraLancamentoLog
    {
        public GeraLancamentoLog(Guid idLancamento)
        {
            IdLancamento = idLancamento;
        }

        public Guid IdLancamento { get; }
    }
}
