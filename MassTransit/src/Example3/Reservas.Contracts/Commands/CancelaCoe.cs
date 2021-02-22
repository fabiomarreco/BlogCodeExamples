using System;
using System.Collections.Generic;
using System.Text;

namespace Reservas.Contracts.Commands
{
    public class CancelaCoe
    {
        public CancelaCoe(string codigo)
        {
            Codigo = codigo;
        }

        public string Codigo { get; set; }
    }
}
