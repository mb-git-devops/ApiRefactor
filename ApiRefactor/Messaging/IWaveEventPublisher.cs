using ApiRefactor.Contracts.Events;

namespace ApiRefactor.Messaging;

public interface IWaveEventPublisher
{
    Task PublishUpsertedAsync(WaveUpserted integrationEvent, CancellationToken cancellationToken = default);
}
