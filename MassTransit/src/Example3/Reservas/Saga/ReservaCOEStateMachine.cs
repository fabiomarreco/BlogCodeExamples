using System;
using Automatonymous;
using ContaCorrente.Contracts.Commands;
using ContaCorrente.Contracts.Events;
using Email.Contracts;
using MassTransit.MongoDbIntegration.Saga;
using MassTransit.Saga;
using MongoDB.Bson.Serialization.Attributes;
using Reservas.Contracts.Commands;
using Reservas.Contracts.Events;

namespace Reservas.Saga
{
    [BsonIgnoreExtraElements]
    public class ChaveReserva
    {
        public int? Cliente { get; set; }
        public string CodigoCOE { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ReservaCOEState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }

        public ChaveReserva Chave { get; set; }

        public Guid? IdEfetivacao { get; set; }

        public DateTime? HoraCriacao { get; set; }

        public decimal ValorReservado { get; set; }

        public string CurrentState { get; set; }
        public int Version { get; set; }

        public string MotivoCancelamento { get; set; }
    }


    public class ReservaCOEStateMachine : MassTransitStateMachine<ReservaCOEState>
    {
        public ReservaCOEStateMachine()
        {
            InstanceState(s => s.CurrentState);


            Event(() => CriaReserva,
                    x => x.CorrelateBy(
                        res => res.Chave,
                        ctx => new ChaveReserva() { Cliente = ctx.Message.Cliente, CodigoCOE = ctx.Message.CodigoCOE })
                            .SelectId(ctx => Guid.NewGuid()));


            Event(() => CancelaReserva, x => x.CorrelateById(ctx => ctx.Message.IdReserva));

            Event(() => COECancelado, x => x.CorrelateBy(ctx => ctx.Chave.CodigoCOE, s => s.Message.CodigoCOE));
            Event(() => PeriodoReservaConcluido, x => x.CorrelateBy(ctx => ctx.Chave.CodigoCOE, s => s.Message.CodigoCOE));

            Event(() => SaldoAtualizado, x => x.CorrelateBy(ctx => ctx.Chave.Cliente, s => s.Message.Conta));
            Event(() => PegaReserva, x =>
            {
                x.CorrelateById(ctx => ctx.Message.IdReserva);
                x.OnMissingInstance(a => a.Execute(b => b.Respond(PegaReservaCoeResp.NaoEncontrada)));
            });



            Initially(
                    When(CriaReserva)
                        .Then(ctx =>
                        {
                            ctx.Instance.HoraCriacao = DateTime.Now;
                            ctx.Instance.Chave = new ChaveReserva()
                            {
                                Cliente = ctx.Data.Cliente,
                                CodigoCOE = ctx.Data.CodigoCOE
                            };

                            ctx.Instance.ValorReservado = ctx.Data.Valor;
                            //alguma outra atividade....
                        })
                        .Publish(ctx => new ReservaCriada(ctx.CorrelationId.Value, ctx.Data.Cliente, ctx.Data.CodigoCOE, DateTime.Now, ctx.Data.Valor))
                        .Publish(ctx => new EnviaEmail(ctx.Instance.Chave.Cliente.Value, $"Reserva Criada em R${ ctx.Instance.ValorReservado}"))
                        .TransitionTo(Reservado));


            During(Reservado,
                When(CriaReserva)
                    .Publish(ctx => new ReservaRejeitada($"Já existe reserva para este cliente com id {ctx.Instance.CorrelationId}")),

                When(SaldoAtualizado)
                    .If(ctx => ctx.Instance.ValorReservado > ctx.Data.Saldo,
                        ctx => ctx.Publish(p => new EnviaEmail(p.Data.Conta, $"O cliente não possui saldo suficiente (minimo={p.Instance.ValorReservado})"))
                                   .TransitionTo(ComPendencias)),



                When(PeriodoReservaConcluido)
                    .Then(x =>
                   {
                       //alguma atividade 
                   })
                    .Publish(x => new ReservaEfetivada(x.Instance.CorrelationId))
                    .Publish(x => new GeraLancamento(x.Instance.Chave.Cliente.Value, -x.Instance.ValorReservado))
                    .Publish(x => new EnviaEmail(x.Instance.Chave.Cliente.Value, "Parabéns, agora voce tem um COE"))
                    .TransitionTo(Efetivado)
                    .Finalize());

            During(ComPendencias,
                When(SaldoAtualizado)
                    .If(ctx => ctx.Instance.ValorReservado <= ctx.Data.Saldo,
                        x => x.TransitionTo(Reservado)),

                When(PeriodoReservaConcluido)
                    .Then(x => x.Instance.MotivoCancelamento = "saldo insuficiente no fim da reserva")
                    .Publish(x => new EnviaEmail(x.Instance.Chave.Cliente.Value, "Você perdeu a oportunidade de ter um COE por saldo insuficiente"))
                        .TransitionTo(Cancelado)
                        .Finalize()
                    );


            DuringAny(
                When(CancelaReserva)
                    //remove do banco ?
                    .Then(x => x.Instance.MotivoCancelamento = "Cancelado pelo cliente")
                        .TransitionTo(Cancelado)
                        .Finalize(),

                    When(COECancelado)
                        .Then(x =>
                        {
                            x.Instance.MotivoCancelamento = "COE Cancelado";
                            //fazer algum cancelamento no banco ... ?
                        })
                        .Publish(x => new ReservaCancelada(x.Instance.CorrelationId))
                        .TransitionTo(Cancelado)
                        .Finalize(),

                    When(PegaReserva)
                        .Respond(ctx => new PegaReservaCoeResp(ctx.Instance.CorrelationId, ctx.Instance.Chave.Cliente.Value, ctx.Instance.ValorReservado, ctx.Instance.Chave.CodigoCOE, ctx.Instance.CurrentState, ctx.Instance.HoraCriacao.Value))
                        );


            SetCompletedWhenFinalized();

        }



        public State ComPendencias { get; set; }
        public State Reservado { get; set; }
        public State Efetivado { get; set; }
        public State Cancelado { get; set; }




        //public Request<ReservaCOEState, PegaSaldoContas, PegaSaldoContasResp> PegaSaldo { get; private set; }
        //public Event<AlteraReserva> AlteraReserva { get; private set; }

        #region Commands
        public Event<CriaReserva> CriaReserva { get; private set; }
        public Event<PegaReservaCoe> PegaReserva { get; private set; }
        public Event<CancelaReserva> CancelaReserva { get; private set; }
        #endregion

        #region Events
        public Event<PeriodoReservaCOEConcluido> PeriodoReservaConcluido { get; private set; }
        public Event<SaldoAtualizado> SaldoAtualizado { get; private set; }
        public Event<COECancelado> COECancelado { get; private set; }
        #endregion
    }
}
