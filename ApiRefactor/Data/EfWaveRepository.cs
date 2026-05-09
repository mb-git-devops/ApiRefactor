using ApiRefactor.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiRefactor.Data;

public sealed class EfWaveRepository(WavesDbContext db) : IWaveRepository
{
    public async Task<IReadOnlyList<Wave>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await db.Waves
            .AsNoTracking()
            .OrderBy(w => w.WaveDate)
            .Select(w => ToDomain(w))
            .ToListAsync(cancellationToken);
    }

    public async Task<Wave?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await db.Waves.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<bool> UpsertAsync(Wave wave, CancellationToken cancellationToken = default)
    {
        var existing = await db.Waves.FindAsync([wave.Id], cancellationToken);
        if (existing is null)
        {
            db.Waves.Add(new WaveEntity { Id = wave.Id, Name = wave.Name, WaveDate = wave.WaveDate });
            await db.SaveChangesAsync(cancellationToken);
            return true;
        }

        existing.Name = wave.Name;
        existing.WaveDate = wave.WaveDate;
        await db.SaveChangesAsync(cancellationToken);
        return false;
    }

    private static Wave ToDomain(WaveEntity e) =>
        new() { Id = e.Id, Name = e.Name, WaveDate = e.WaveDate };
}
