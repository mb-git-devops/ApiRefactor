using Microsoft.EntityFrameworkCore;

namespace ApiRefactor.Data;

public sealed class WavesDbContext(DbContextOptions<WavesDbContext> options) : DbContext(options)
{
    public DbSet<WaveEntity> Waves => Set<WaveEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WaveEntity>(entity =>
        {
            entity.ToTable("waves");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasConversion<string>();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(500);
            entity.Property(e => e.WaveDate).IsRequired();
        });
    }
}
