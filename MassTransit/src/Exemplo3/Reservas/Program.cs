using MassTransit;
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


            var address = new Uri("rabbitmq://localhost/teste");

            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(address, x =>
                {
                    x.Username("guest");
                    x.Password("guest");
                });




            })
        }
    }
}
