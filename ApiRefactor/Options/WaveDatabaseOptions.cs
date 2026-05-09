namespace ApiRefactor.Options;

public sealed class WaveDatabaseOptions
{
    public const string SectionName = "WaveDatabase";

    /// <summary>SQLite connection string, e.g. Data Source=App_Data/waves.db</summary>
    public string ConnectionString { get; set; } = "Data Source=App_Data/waves.db";
}
