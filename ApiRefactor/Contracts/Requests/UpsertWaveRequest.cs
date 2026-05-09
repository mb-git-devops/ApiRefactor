namespace ApiRefactor.Contracts.Requests;

/// <summary>Request body for creating or updating a wave. Omit Id to assign a new identifier server-side.</summary>
public sealed class UpsertWaveRequest
{
    /// <summary>When null, a new wave identifier is generated.</summary>
    public Guid? Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime WaveDate { get; set; }
}
