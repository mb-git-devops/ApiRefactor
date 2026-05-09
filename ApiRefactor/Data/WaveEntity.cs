namespace ApiRefactor.Data;

public sealed class WaveEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime WaveDate { get; set; }
}
