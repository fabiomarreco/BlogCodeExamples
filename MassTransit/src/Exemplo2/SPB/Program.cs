using MassTransit;
using GreenPipes;
using Serilog;
using SPB.Consumidores;
using SPB.Contracts;
using System;
using SPB.Contracts.Commands;

namespace SPB
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Verbose()
                .CreateLogger();


            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.UseSerilog(Log.Logger);

                    cfg.Host(new Uri("rabbitmq://localhost/teste"), q =>
                    {
                        q.Username("guest");
                        q.Password("guest");
                    });

                    cfg.ReceiveEndpoint(SPBQueues.CommandQueue, x =>
                    {
                        x.ExecuteActivityHost<EnviaTransacaoConsumer, EnviaTransacao>();
                    });
                });


            bus.Start();


            Console.WriteLine("Press any key");
            Console.ReadKey();

            bus.Stop();
        }
    }
}
