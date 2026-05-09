using ApiRefactor.Contracts.Events;
using ApiRefactor.Contracts.Requests;
using ApiRefactor.Contracts.Responses;
using ApiRefactor.Data;
using ApiRefactor.Messaging;
using ApiRefactor.Models;

namespace ApiRefactor.Services;

public sealed class WaveService(
    IWaveRepository repository,
    IWaveEventPublisher eventPublisher,
    ILogger<WaveService> logger) : IWaveService
{
    public async Task<WavesListResponse> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var waves = await repository.GetAllAsync(cancellationToken);
        return new WavesListResponse
        {
            Items = waves.Select(w => new WaveResponse(w.Id, w.Name, w.WaveDate)).ToList()
        };
    }

    public Task<Wave?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public async Task<(Wave Wave, bool WasInserted)> UpsertAsync(
        UpsertWaveRequest request,
        CancellationToken cancellationToken = default)
    {
        var id = request.Id ?? Guid.NewGuid();
        var wave = new Wave
        {
            Id = id,
            Name = request.Name,
            WaveDate = request.WaveDate
        };

        var wasInserted = await repository.UpsertAsync(wave, cancellationToken);

        try
        {
            await eventPublisher.PublishUpsertedAsync(
                new WaveUpserted(wave.Id, wave.Name, wave.WaveDate, wasInserted, DateTimeOffset.UtcNow),
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish {Event} for wave {WaveId}", nameof(WaveUpserted), wave.Id);
        }

        return (wave, wasInserted);
    }
}
