using ContaCorrente.Contracts;
using ContaCorrente.Contracts.Commands;
using ContaCorrente.Contracts.Events;
using ContaCorrente.Contracts.Query;
using MassTransit;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Exemplo.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Information().CreateLogger();


            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost/teste"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                #region handlers
                cfg.ReceiveEndpoint(r => r.Handler<SaldoAtualizado>(
                    async sd => Log.Information("EVENT: {@message}", sd.Message)));

                cfg.ReceiveEndpoint(r => r.Handler<Fault<GeraLancamento>>(
                    async sd => Log.Information("FAULT: {@message}", new { sd.MessageId, sd.Message.Exceptions[0].Message })));

                cfg.ReceiveEndpoint(r => r.Handler<Pong>(
                    async sd => Log.Information("Pong: {@message}", sd.Message)));
                #endregion
            });

            bus.Start();

            Console.WriteLine("Bus started.");

            while (Execute(bus).Result)
            {
            }

            bus.Stop();

            
        }


        static async Task<bool> Execute(IBus bus)
        {
            var strCmd = Console.ReadLine().Split(' ');
            var fstCmd = strCmd.FirstOrDefault();
            if (string.IsNullOrEmpty(fstCmd))
                return true;


            var debita = await bus.GetSendEndpoint(GetQueueUri(bus, ContaCorrenteQueues.CommandQueue));
            var pegaSaldos = bus.CreateRequestClient<PegaSaldoContas, PegaSaldoContasResp>
                            (GetQueueUri(bus, ContaCorrenteQueues.QueryQueue), TimeSpan.FromMinutes(1));
            try
            {
                switch (fstCmd.ToLowerInvariant())
                {
                    case "exit": return false;

                    case "lanca":// lanca [clientid] [valor]
                        var conta = int.Parse(strCmd[1]); var valor = decimal.Parse(strCmd[2]);
                        await debita.Send(new GeraLancamento(conta, valor)); return true;

                    case "pegasaldos":// pegasaldos 999,123,444
                        

                        var contas = strCmd[1].Split(',').Select(s=> int.Parse(s.Trim())).ToArray();
                        var response = await pegaSaldos.Request(new PegaSaldoContas(contas));
                        Console.WriteLine("Response: " + JsonConvert.SerializeObject(response, Formatting.Indented));
                        return true;


                    case "lanca-stream":
                        var rnd = new Random();

                        while (!Console.KeyAvailable)
                        {
                            var msg = new GeraLancamento(rnd.Next(1, 1000), (decimal)(rnd.NextDouble() * 100000.0 - 10000));
                            Log.Information("Sending: {@msg}" , msg);
                            await debita.Send(msg);
                        }
                        return true;

                    case "ping": await bus.Publish(new Ping()); return true;

                    default: Console.WriteLine($"commands: \n\thelp \n\tpegasaldos 12,323 \n\tlanca 12 100.32 \n\texit \n\tlanca-stream"); return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return true;
            }


        }



        public static Uri GetQueueUri(IBus bus, string queueName)
        {
            return new Uri($"rabbitmq://localhost/teste/{queueName}");
        }

    }
}
