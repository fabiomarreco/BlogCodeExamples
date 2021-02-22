using ContaCorrente.Contracts.Commands;
using System;
using System.Threading.Tasks;
using ContaCorrente.Contracts.Events;
using System.Threading;
using MassTransit.Courier;
using System.Transactions;

namespace ContaCorrente.Consumidores
{

    public class GeraLancamentoActivity : IActivity<GeraLancamento, GeraLancamentoLog>
    {

        public async Task<ExecutionResult> Execute(ExecuteContext<GeraLancamento> context)
        {
            var msg = context.Arguments;

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var novoSaldo = Repositorio.AdicionaTransacao(
                    new Transacao(context.ExecutionId, DateTime.Now, "lancamento", msg.Account, msg.Valor));

                await Task.Delay(1000);
                context.Publish(new SaldoAtualizado(msg.Account, novoSaldo)).Wait();

                var result = context.Completed(new GeraLancamentoLog(context.ExecutionId));
                //var result = context.CompletedWithVariables(new GeraLancamentoLog(context.ExecutionId), ...);
                scope.Complete();
                return result;
            }
        }


        public async Task<CompensationResult> Compensate(CompensateContext<GeraLancamentoLog> context)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var transacaoOriginal = Repositorio.PegaTransacao(context.Log.IdLancamento);

                if (transacaoOriginal == null)
                    return context.Compensated();

                var novoSaldo = Repositorio.AdicionaTransacao(
                    new Transacao(Guid.NewGuid(), 
                        DateTime.Now, 
                        "Estorno " + transacaoOriginal.Id, 
                        transacaoOriginal.Conta, - transacaoOriginal.Valor));


                await context.Publish(new SaldoAtualizado(transacaoOriginal.Conta, novoSaldo));
                scope.Complete();
                return context.Compensated();
            }
        }

    }
}
