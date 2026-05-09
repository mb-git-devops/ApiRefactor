using ApiRefactor.Models;

namespace ApiRefactor.Data;

public interface IWaveRepository
{
    Task<IReadOnlyList<Wave>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Wave?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Persists the wave; returns true if inserted, false if updated.</summary>
    Task<bool> UpsertAsync(Wave wave, CancellationToken cancellationToken = default);
}
