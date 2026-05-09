namespace ApiRefactor.Contracts.Responses;

public sealed class WavesListResponse
{
    public IReadOnlyList<WaveResponse> Items { get; init; } = Array.Empty<WaveResponse>();
}
