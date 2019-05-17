using ContaCorrente.Contracts.Commands;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Dapper;
using System.Threading.Tasks;
using ContaCorrente.Contracts.Events;
using System.Transactions;
using System.Threading;

namespace ContaCorrente.Consumidores
{
    public class GeraLancamentoConsumer : IConsumer<GeraLancamento>
    {
        private const string CONN_STR = "Server=localhost;DataBase=Exemplo.ContaCorrente; Trusted_Connection=true";

        public async Task Consume(ConsumeContext<GeraLancamento> context)
        {
            var msg = context.Message;

            using (var scope = new TransactionScope())
            {
                decimal novoSaldo;
                using (var conn = new SqlConnection(CONN_STR))
                {
                    novoSaldo = conn.ExecuteScalar<decimal>(@"
                    insert into transacao (id, conta, data, descricao, valor) values (@ID, @Conta, @Data, @Descricao, @Valor); 

                    ; merge saldo with (HOLDLOCK) as target
                    using (values (@valor)) as source (value) on target.conta = @Conta
                    when matched then update set Valor = Valor + @Valor
                    when not matched then insert (conta, valor) values (@conta, @valor);

                    select valor from saldo where conta = @conta",
                        new { ID = context.MessageId, Conta = msg.Account, Data = DateTime.Now, Descricao = "Debito", msg.Valor });
                }

                Thread.Sleep(1000);
                context.Publish(new SaldoAtualizado(msg.Account, novoSaldo)).Wait();
                scope.Complete();
            }


        }
    }
}
