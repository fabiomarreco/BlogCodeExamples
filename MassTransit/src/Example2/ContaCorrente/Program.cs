using ContaCorrente.Consumidores;
using ContaCorrente.Consumidores.Query;
using ContaCorrente.Contracts;
using ContaCorrente.Contracts.Commands;
using GreenPipes;
using MassTransit;
using Serilog;
using System;
using System.Threading;

namespace ContaCorrente
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger =
                new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Verbose()
                .CreateLogger();

            var baseUri = new Uri("rabbitmq://localhost/teste");

            var bus = Bus.Factory.CreateUsingRabbitMq(config =>
            {
                config.UseSerilog(Log.Logger);

                config.Host(baseUri, h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                //------------- ACTIVITY  ------------------//
                Uri compensationAddress = null;
                config.ReceiveEndpoint(ContaCorrenteQueues.Compensation, re =>
                {
                    re.CompensateActivityHost<GeraLancamentoActivity, GeraLancamentoLog>();
                    compensationAddress = re.InputAddress;
                });


                config.ReceiveEndpoint(ContaCorrenteQueues.ActivityQueue, re =>
                {
                    //re.UseMessageRetry(o => o.Interval(5, 500));
                    re.ExecuteActivityHost<GeraLancamentoActivity, GeraLancamento>(compensationAddress);
                });



                config.ReceiveEndpoint(ContaCorrenteQueues.QueryQueue, re => re.Consumer<PegaSaldoContasConsumer>());
                config.ReceiveEndpoint(re =>
                {
                    re.Handler<Ping>(h => h.Publish(new Pong(Thread.CurrentThread.ManagedThreadId)));
                });
            });


            bus.Start();

            do { Console.WriteLine("Press q to exit"); }
            while (Console.ReadKey().KeyChar != 'q');


            bus.Stop();
        }
    }
}
