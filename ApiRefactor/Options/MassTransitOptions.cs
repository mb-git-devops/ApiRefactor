namespace ApiRefactor.Options;

public sealed class MassTransitOptions
{
    public const string SectionName = "MassTransit";

    /// <summary>When empty, the app uses the in-memory transport. Set to AzureServiceBus to use Azure Service Bus.</summary>
    public string Transport { get; set; } = string.Empty;

    /// <summary>Azure Service Bus namespace connection string (e.g. from portal or Key Vault).</summary>
    public string? AzureServiceBusConnectionString { get; set; }
}
