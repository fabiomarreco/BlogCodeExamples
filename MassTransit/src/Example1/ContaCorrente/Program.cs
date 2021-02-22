using ContaCorrente.Consumidores;
using ContaCorrente.Consumidores.Query;
using ContaCorrente.Contracts;
using GreenPipes;
using MassTransit;
using Serilog;
using Serilog.Core;
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


            var bus = Bus.Factory.CreateUsingRabbitMq(config =>
            {
                config.UseSerilog(Log.Logger);

                config.Host(new Uri("rabbitmq://localhost/teste"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                config.ReceiveEndpoint(ContaCorrenteQueues.CommandQueue, re =>
                {
                    //re.UseMessageRetry(o => o.Exponential(5, 500));
                    re.Consumer<GeraLancamentoConsumer>();
                });

                config.ReceiveEndpoint(ContaCorrenteQueues.QueryQueue, re =>
                {
                    re.Consumer<PegaSaldoContasConsumer>();
                });


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
