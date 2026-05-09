using ApiRefactor.Contracts.Events;
using MassTransit;

namespace ApiRefactor.Messaging;

public sealed class MassTransitWaveEventPublisher(IPublishEndpoint publishEndpoint) : IWaveEventPublisher
{
    public Task PublishUpsertedAsync(WaveUpserted integrationEvent, CancellationToken cancellationToken = default) =>
        publishEndpoint.Publish(integrationEvent, cancellationToken);
}
