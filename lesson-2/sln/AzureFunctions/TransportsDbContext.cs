using AzureFunctions.Models;

using Microsoft.EntityFrameworkCore;

namespace AzureFunctions;

public class TransportsDbContext: DbContext
{
    public DbSet<TransportEntity> Transports { get; set; }
    
    public TransportsDbContext(DbContextOptions<TransportsDbContext> options) : base(options)
    {
    }
}
