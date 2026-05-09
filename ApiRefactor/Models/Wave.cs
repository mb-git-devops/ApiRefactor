namespace ApiRefactor.Models;

public sealed class Wave
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime WaveDate { get; init; }
}
