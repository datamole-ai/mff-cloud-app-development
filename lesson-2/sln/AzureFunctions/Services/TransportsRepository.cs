using AzureFunctions.Models;

using Microsoft.EntityFrameworkCore;

namespace AzureFunctions.Services;

public class TransportsRepository(TransportsDbContext dbContext)
{
    public async Task SaveTransportAsync(Transport transport)
    {
        dbContext.Transports.Add(TransportEntity.FromTransport(transport));
        await dbContext.SaveChangesAsync();
    }

    public async Task<TransportEntity?> FindTransportAsync(DateOnly date, string facilityId, string parcelId)
    {
        return await dbContext.Transports.FindAsync(
            date,
            facilityId,
            parcelId
        );
    }

    public async Task<IList<TransportEntity>> GetTransportsAsync(DateOnly date)
    {
        return await dbContext.Transports.Where(transport => transport.TransportedDate == date).ToListAsync();
    }
}

