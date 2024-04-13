using AzureFunctions.Models;

using Microsoft.EntityFrameworkCore;

namespace AzureFunctions.Services;

public class TransportsDbContext : DbContext
{
    public DbSet<TransportEntity> Transports { get; set; }

    public TransportsDbContext(DbContextOptions<TransportsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransportEntity>()
            .HasKey(transport => new { transport.TransportedDate, transport.FacilityId, transport.ParcelId });
    }
}
