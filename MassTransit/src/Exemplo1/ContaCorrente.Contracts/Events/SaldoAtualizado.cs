using System;
using System.Collections.Generic;
using System.Text;

namespace ContaCorrente.Contracts.Events
{
    public class SaldoAtualizado
    {
        public SaldoAtualizado(int conta, decimal saldo)
        {
            Conta = conta;
            Saldo = saldo;
        }

        public int Conta { get; }
        public decimal Saldo { get; }
    }
}
