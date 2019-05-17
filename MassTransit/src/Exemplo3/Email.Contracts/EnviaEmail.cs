using System;

namespace Email.Contracts
{
    public class EnviaEmail
    {
        public EnviaEmail(string destinatario, string conteudo)
        {
            Destinatario = destinatario;
            Conteudo = conteudo;
        }

        public string Destinatario { get; }
        public string Conteudo { get; }
    }
}
