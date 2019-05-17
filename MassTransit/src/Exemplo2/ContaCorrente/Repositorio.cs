using System;
using System.Data.SqlClient;
using Dapper;

namespace ContaCorrente.Consumidores
{
    public class Repositorio
    {
        private const string CONN_STR = "Server=localhost;DataBase=Exemplo.ContaCorrente; Trusted_Connection=true";

        public static decimal AdicionaTransacao(Transacao transacao)
        {
            using (var conn = new SqlConnection(CONN_STR))
            {
                var novoSaldo = conn.ExecuteScalar<decimal>(@"
                    insert into transacao (id, conta, data, descricao, valor) values (@Id, @Conta, @Data, @Descricao, @Valor); 

                    ; merge saldo with (HOLDLOCK) as target
                    using (values (@valor)) as source (value) on target.conta = @Conta
                    when matched then update set Valor = Valor + @Valor
                    when not matched then insert (conta, valor) values (@conta, @valor);

                    select valor from saldo where conta = @conta",
                    transacao);

                return novoSaldo;
            }
        }


        public static Transacao PegaTransacao(Guid id)
        {
            using (var con = new SqlConnection(CONN_STR))
            {
                return con.QueryFirstOrDefault<Transacao>
                    ("select id, data, descricao, conta, valor from transacao where id = @Id", new { Id = id });
            }
        }

    }



    public class Transacao
    {
        public Transacao(Guid id, DateTime data, string descricao, int conta, decimal valor)
        {
            Id = id;
            Data = data;
            Descricao = descricao;
            Conta = conta;
            Valor = valor;
        }

        public Guid Id { get; }
        public DateTime Data { get; }
        public string Descricao { get; }
        public int Conta { get; }
        public decimal Valor { get; }
    }
}
