using System;
using System.Collections.Generic;
using System.Text;

namespace SPB.Contracts.Commands
{
    public class EnviaTransacao
    {
        public EnviaTransacao(int contaDestino, decimal valor, string descricao)
        {
            ContaDestino = contaDestino;
            Valor = valor;
            Descricao = descricao;
        }

        public int ContaDestino { get; }
        public decimal Valor { get; }
        public string Descricao { get; }
    }
}
