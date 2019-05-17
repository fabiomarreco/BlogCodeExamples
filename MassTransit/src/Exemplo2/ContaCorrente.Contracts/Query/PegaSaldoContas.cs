using System;
using System.Collections.Generic;
using System.Text;

namespace ContaCorrente.Contracts.Query
{
    public class PegaSaldoContas
    {
        public PegaSaldoContas(IReadOnlyCollection<int> contas)
        {
            Contas = contas;
        }

        public IReadOnlyCollection<int> Contas { get; }
    }

    public class PegaSaldoContasResp
    {
        public PegaSaldoContasResp(IReadOnlyDictionary<int, decimal> saldos)
        {
            Saldos = saldos;
        }

        public IReadOnlyDictionary<int, decimal> Saldos { get; }
    }
}
