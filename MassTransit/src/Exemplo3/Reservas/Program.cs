using MassTransit;
using MassTransit.MongoDbIntegration.Saga;
using MongoDB.Driver;
using Reservas.Contracts;
using Reservas.Contracts.Commands;
using Reservas.Contracts.Events;
using Reservas.Saga;
using Serilog;
using System;

namespace Reservas
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();


            var address = new Uri("rabbitmq://localhost/exemplo3");

            var repository = new MongoDbSagaRepository<ReservaCOEState>("mongodb://root:example@localhost:27017", "ReservaCOE");

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.UseSerilog();

                var host = cfg.Host(address, x =>
                {
                    x.Username("guest");
                    x.Password("guest");
                });


                cfg.ReceiveEndpoint("coe-commands",
                    e =>
                    {
                        e.Handler<CancelaCoe>(c => c.Publish(new COECancelado(c.Message.Codigo)));
                        e.Handler<ConcluiReservaCoe>(c => c.Publish(new PeriodoReservaCOEConcluido(c.Message.Codigo)));
                    });


                cfg.ReceiveEndpoint(host, ReservasQueues.StateMachine, e =>
                {
                    e.StateMachineSaga(new ReservaCOEStateMachine(), repository);
                });

            });


            bus.Start();

            while (Console.ReadKey().KeyChar != 'q')
                Console.WriteLine("Press q to continue");


            bus.Stop();

        }
    }
}
