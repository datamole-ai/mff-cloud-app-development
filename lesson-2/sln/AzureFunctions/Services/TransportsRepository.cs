using System.Threading.Tasks;

using AzureFunctions.Models;

namespace AzureFunctions.Services;

public class TransportsRepository(TransportsDbContext dbContext)
{
    public async Task SaveTransportAsync(Transport transport)
    {
        dbContext.Transports.Add(TransportEntity.FromTransport(transport));
        await dbContext.SaveChangesAsync();
    }
}

