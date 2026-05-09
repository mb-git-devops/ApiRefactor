namespace ApiRefactor.Contracts.Events;

/// <summary>Published after a wave is successfully persisted (insert or update).</summary>
public sealed record WaveUpserted(
    Guid WaveId,
    string Name,
    DateTime WaveDate,
    bool WasInserted,
    DateTimeOffset OccurredAt);
