namespace ApiRefactor.Options;

/// <summary>Demo-only shared secret for Bearer authentication. Replace with real OIDC/JWT in production.</summary>
public sealed class DummyAuthOptions
{
    public const string SectionName = "DummyAuth";
    public string BearerToken { get; set; } = string.Empty;
}
