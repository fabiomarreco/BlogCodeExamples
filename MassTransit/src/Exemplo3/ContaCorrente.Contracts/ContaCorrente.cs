using System;
using System.Collections.Generic;
using System.Text;

namespace ContaCorrente.Contracts
{
    public static class ContaCorrenteQueues
    {
        public const string CommandQueue = "Conta-Corrente";
        public const string QueryQueue = "conta-Corrente-Consulta";
    }



    public class Ping
    {

    }

    public class Pong
    {
        public Pong(int threadId)
        {
            ThreadId = threadId;
        }

        public int ThreadId { get; }
    }
}
