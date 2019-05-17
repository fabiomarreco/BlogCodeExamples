using MassTransit;
using MassTransit.Courier;
using SPB.Contracts.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SPB.Consumidores
{
    public class EnviaTransacaoConsumer : ExecuteActivity<EnviaTransacao>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<EnviaTransacao> context)
        {
            ///fake
            if (context.Arguments.Descricao.Contains("poison", StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidOperationException("Rejeitado pelo banco central");

            return context.CompletedWithVariables(new { SPBId = Guid.NewGuid() } );
        }

    }
}
