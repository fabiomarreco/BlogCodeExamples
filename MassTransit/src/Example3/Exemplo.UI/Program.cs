using ContaCorrente.Contracts;
using ContaCorrente.Contracts.Commands;
using ContaCorrente.Contracts.Events;
using ContaCorrente.Contracts.Query;
using Email.Contracts;
using MassTransit;
using Newtonsoft.Json;
using Reservas.Contracts;
using Reservas.Contracts.Commands;
using Reservas.Contracts.Events;
using Serilog;
using System;
using System.Globalization;
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
                cfg.Host(new Uri("rabbitmq://localhost/exemplo3"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                void AddHandler<T> () where T: class=> 
                    cfg.ReceiveEndpoint(r => r.Handler<T>(sd => {
                        Log.Information("EVENT: {@message}", sd.Message);
                        return sd.ConsumeCompleted; }));


                AddHandler<SaldoAtualizado>();
                AddHandler<COECancelado>();
                AddHandler<ReservaCancelada>();
                AddHandler<ReservaCriada>();
                AddHandler<ReservaEfetivada>();
                AddHandler<ReservaRejeitada>();
                AddHandler<PeriodoReservaCOEConcluido>();
                AddHandler<EnviaEmail>();
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

            var pegaSaldos = bus.CreateRequestClient<PegaSaldoContas>
                                    (GetQueueUri(ContaCorrenteQueues.QueryQueue), TimeSpan.FromMinutes(1));


            var pegaReserva = bus.CreateRequestClient<PegaReservaCoe>
                                    (GetQueueUri(ReservasQueues.StateMachine), TimeSpan.FromMinutes(1));


            var debita = await bus.GetSendEndpoint(GetQueueUri(ContaCorrenteQueues.CommandQueue));

            try
            {
                switch (fstCmd.ToLowerInvariant())
                {
                    case "exit": return false;
                    case "lanca":
                        var conta = int.Parse(strCmd[1]); var valor = decimal.Parse(strCmd[2],NumberFormatInfo.InvariantInfo);
                        await debita.Send(new GeraLancamento(conta, valor)); return true;
                    case "pegasaldos":
                        var contas = strCmd[1].Split(',').Select(s=> int.Parse(s.Trim())).ToArray();
                        var response = await pegaSaldos.GetResponse<PegaSaldoContasResp>(new PegaSaldoContas(contas));
                        Console.WriteLine("Response: " + JsonConvert.SerializeObject(response, Formatting.Indented));
                        return true;



                    case "cancela-coe": await bus.Publish(new CancelaCoe(strCmd[1])); return true;
                    case "cancela-reserva": await bus.Publish(new CancelaReserva(Guid.Parse(strCmd[1]))); return true;
                    case "conclui-reserva": await bus.Publish(new ConcluiReservaCoe(strCmd[1])); return true;
                    case "cria-reserva": await bus.Publish(new CriaReserva(int.Parse(strCmd[1]), strCmd[2], decimal.Parse(strCmd[3], NumberFormatInfo.InvariantInfo))); return true;
                    case "pega-reserva":
                        var resp = await pegaReserva.GetResponse<PegaReservaCoeResp>(new PegaReservaCoe(Guid.Parse(strCmd[1])));
                        Console.WriteLine("Response: " + JsonConvert.SerializeObject(resp, Formatting.Indented));
                        return true;


                    default: Console.WriteLine(@"commands:
    exit
    lanca 12 100.2             <- Adiciona Transacao 
    pegasaldos 12,34           <- Consulta transacao das contas separadas por virgula
    cancela-coe  ABC           <- Cancela code codigo ABC
    cancela-reserva 123s23     <- Cancela reserva com IdReserva e motivo
    conclui-reserva ABC        <- Conclui o periodo de reserva do coe
    pega-reserva 123s23        <- Pega detalhes da reserva com ID
    cria-reserva  12 ABC 1000  <- Cria reserva para o cliente /coe /valor "); return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return true;
            }


        }



        public static Uri GetQueueUri(string queueName)
        {
            return new Uri($"rabbitmq://localhost/exemplo3/{queueName}");
        }

    }
}
