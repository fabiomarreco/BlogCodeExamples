using System;
using System.Collections.Generic;
using System.Text;

namespace Reservas.Contracts.Events
{
    public class COECancelado
    {
        public COECancelado(string codigoCOE)
        {
            CodigoCOE = codigoCOE;
        }

        public string CodigoCOE { get; }
    }
}
