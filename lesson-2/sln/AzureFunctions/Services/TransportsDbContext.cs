using AzureFunctions.Models;

using Microsoft.EntityFrameworkCore;

namespace AzureFunctions.Services;

public class TransportsDbContext(DbContextOptions<TransportsDbContext> options) : DbContext(options)
{
    public DbSet<TransportEntity> Transports { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransportEntity>()
            .HasKey(transport => new { transport.TransportedDate, transport.FacilityId, transport.ParcelId });
    }
}
