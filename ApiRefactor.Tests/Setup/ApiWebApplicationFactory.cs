using ApiRefactor.Messaging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ApiRefactor.Tests.Setup;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
    public const string TestBearerToken = "test-dummy-token";

    private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"waves-test-{Guid.NewGuid():N}.db");

    public RecordingWaveEventPublisher RecordingPublisher { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["WaveDatabase:ConnectionString"] = $"Data Source={_databasePath}",
                    ["DummyAuth:BearerToken"] = TestBearerToken
                });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IWaveEventPublisher>();
            services.AddSingleton<IWaveEventPublisher>(RecordingPublisher);
        });
    }

    public new void Dispose()
    {
        base.Dispose();
        try
        {
            if (File.Exists(_databasePath))
            {
                File.Delete(_databasePath);
            }
        }
        catch
        {
            // ignore temp file cleanup failures on Windows locks
        }
    }
}
