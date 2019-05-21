using System;

namespace Email.Contracts
{
    public class EnviaEmail
    {
        public EnviaEmail(int cliente, string conteudo)
        {
            Cliente = cliente;
            Conteudo = conteudo;
        }

        public int Cliente { get; }
        public string Conteudo { get; }
    }
}
