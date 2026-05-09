using ApiRefactor.Contracts.Events;
using MassTransit;

namespace ApiRefactor.Messaging.Consumers;

/// <summary>Consumes <see cref="WaveUpserted"/> integration events from the message bus.</summary>
public sealed class WaveUpsertedConsumer(ILogger<WaveUpsertedConsumer> logger) : IConsumer<WaveUpserted>
{
    public Task Consume(ConsumeContext<WaveUpserted> context)
    {
        var m = context.Message;
        logger.LogInformation(
            "Consumed {Event}: WaveId={WaveId} Name={Name} WaveDate={WaveDate} WasInserted={WasInserted} OccurredAt={OccurredAt}",
            nameof(WaveUpserted),
            m.WaveId,
            m.Name,
            m.WaveDate,
            m.WasInserted,
            m.OccurredAt);
        return Task.CompletedTask;
    }
}
