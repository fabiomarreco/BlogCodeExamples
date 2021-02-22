using ContaCorrente.Contracts.Query;
using Dapper;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContaCorrente.Consumidores.Query
{
    public class PegaSaldoContasConsumer : IConsumer<PegaSaldoContas>
    {
        private const string CONN_STR = "Server=localhost;DataBase=Exemplo.ContaCorrente; Trusted_Connection=true";

        public async Task Consume(ConsumeContext<PegaSaldoContas> context)
        {
            Dictionary<int, decimal> saldos;
            using (var conn = new SqlConnection(CONN_STR))
            {
                 saldos = conn.Query(@"select conta, valor from saldo where conta in @Contas",
                    new { Contas = context.Message.Contas.ToList() })
                    .ToDictionary(x=> (int)x.conta, x=> (decimal)x.valor);

            }

            await context.RespondAsync(new PegaSaldoContasResp(saldos));


        }
    }
}
