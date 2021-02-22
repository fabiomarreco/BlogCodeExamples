using ContaCorrente.Contracts;
using ContaCorrente.Contracts.Commands;
using ContaCorrente.Contracts.Events;
using ContaCorrente.Contracts.Query;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using Newtonsoft.Json;
using Serilog;
using SPB.Contracts;
using SPB.Contracts.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Exemplo.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Information().CreateLogger();

            Task LogInfoAsync (string msg, params object[] values)
            {
                Log.Information(msg, values);
                return Task.CompletedTask;
            }

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost/teste"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint(r => r.Handler<SaldoAtualizado>(
                    sd => LogInfoAsync("EVENT: {@message}", sd.Message)));

                cfg.ReceiveEndpoint(r => r.Handler<Fault<GeraLancamento>>(
                    sd => LogInfoAsync("FAULT: {@message}", new { sd.MessageId, sd.Message.Exceptions[0].Message })));

                cfg.ReceiveEndpoint(r => r.Handler<Pong>(
                    sd => LogInfoAsync("Pong: {@message}", sd.Message)));

                cfg.ReceiveEndpoint("routingSlipLogs",
                    r =>
                    {
                        r.Handler<RoutingSlipCompleted>(h => LogInfoAsync("RS COMPLETED: {@event}", h.Message));
                        r.Handler<RoutingSlipFaulted>(h => LogInfoAsync("RS Faulted!"));
                        r.Handler<RoutingSlipActivityCompleted>(h => LogInfoAsync("ACT Completed: {@event}", new { h.Message.ActivityName, h.Message.Data, h.Message.Variables }));
                        r.Handler<RoutingSlipActivityFaulted>(h => LogInfoAsync("ACT Faulted: {@event}", h.Message.ExceptionInfo.Message));
                        r.Handler<RoutingSlipActivityCompensated>(h => LogInfoAsync("ACT Compensated: {@event}", new { h.Message.ActivityName, h.Message.Data } ));
                    });
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

            var pegaSaldos = 
               // bus.CreateRequestClient<PegaSaldoContas, PegaSaldoContasResp>
               bus.CreateRequestClient<PegaSaldoContas>
                (GetQueueUri(bus, ContaCorrenteQueues.QueryQueue), TimeSpan.FromMinutes(1));

            try
            {
                switch (fstCmd.ToLowerInvariant())
                {
                    case "exit": return false;
                    case "lanca": // [conta] [valor] [desc?]
                        var conta = int.Parse(strCmd[1]);
                        var valor = decimal.Parse(strCmd[2]);
                        var desc = (strCmd.Length > 3) ? strCmd[3] : "Lancamento";

                        await EnviaLancamento(bus, conta, desc, valor);
                        return true;

                    case "pegasaldos":
                        var contas = strCmd[1].Split(',').Select(s => int.Parse(s.Trim())).ToArray();
                        var response = await pegaSaldos.GetResponse<PegaSaldoContasResp>(new PegaSaldoContas(contas));
                        Console.WriteLine("Response: " + JsonConvert.SerializeObject(response, Formatting.Indented));
                        return true;

                    case "ping": await bus.Publish(new Ping()); return true;

                    default: Console.WriteLine($"commands: \n\thelp \n\tpegasaldos 12,323 \n\tlanca 12 100.32 \n\texit"); return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return true;
            }


        }


        public static async Task EnviaLancamento(IBus bus, int conta, string desc, decimal valor)
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());

            builder.AddActivity("GeraLancamento", GetQueueUri(bus, ContaCorrenteQueues.ActivityQueue),
                new GeraLancamento(conta, valor));

            builder.AddActivity("EnvioSPB", GetQueueUri(bus, SPBQueues.CommandQueue),
                new EnviaTransacao(conta, valor, desc));


            builder.AddSubscription(GetQueueUri(bus, "routingSlipLogs"), RoutingSlipEvents.All);

            var routingSlip = builder.Build();

            await bus.Execute(routingSlip);

        }


        public static Uri GetQueueUri (IBus bus, string queueName)
        {
            return new Uri($"rabbitmq://localhost/teste/{queueName}");
        }

    }
}
