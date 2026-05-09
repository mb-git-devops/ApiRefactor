using ApiRefactor.Contracts.Requests;
using ApiRefactor.Contracts.Responses;
using ApiRefactor.Models;

namespace ApiRefactor.Services;

public interface IWaveService
{
    Task<WavesListResponse> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Wave?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(Wave Wave, bool WasInserted)> UpsertAsync(UpsertWaveRequest request, CancellationToken cancellationToken = default);
}
