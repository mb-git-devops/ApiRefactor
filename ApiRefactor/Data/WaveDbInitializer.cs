using ApiRefactor.Options;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApiRefactor.Data;

public sealed class WaveDbInitializer(IOptions<WaveDatabaseOptions> options)
{
    private readonly WaveDatabaseOptions _options = options.Value;

    public void EnsureDatabaseDirectoryExists()
    {
        var builder = new SqliteConnectionStringBuilder(_options.ConnectionString);
        var directory = Path.GetDirectoryName(Path.GetFullPath(builder.DataSource));
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public async Task EnsureCreatedAsync(WavesDbContext db, CancellationToken cancellationToken = default)
    {
        EnsureDatabaseDirectoryExists();
        await db.Database.EnsureCreatedAsync(cancellationToken);
    }
}
