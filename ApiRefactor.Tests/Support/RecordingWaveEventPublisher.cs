using ApiRefactor.Contracts.Events;
using ApiRefactor.Messaging;

namespace ApiRefactor.Tests.Support;

public sealed class RecordingWaveEventPublisher : IWaveEventPublisher
{
    private readonly object _lock = new();
    private readonly List<WaveUpserted> _events = new();

    public IReadOnlyList<WaveUpserted> Events
    {
        get
        {
            lock (_lock)
            {
                return _events.ToList();
            }
        }
    }

    public Task PublishUpsertedAsync(WaveUpserted integrationEvent, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _events.Add(integrationEvent);
        }

        return Task.CompletedTask;
    }

    public void Clear()
    {
        lock (_lock)
        {
            _events.Clear();
        }
    }
}
