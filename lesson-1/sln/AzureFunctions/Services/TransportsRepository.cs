using AzureFunctions.Models;

using Microsoft.EntityFrameworkCore;

namespace AzureFunctions.Services;

public class TransportsRepository(TransportsDbContext dbContext)
{
    public async Task SaveTransportAsync(Transport transport)
    {
        var transportEntity = TransportEntity.FromTransport(transport);

        if (await dbContext.Transports.FindAsync(transportEntity.TransportedDate, transportEntity.FacilityId, transportEntity.ParcelId) is not null)
        {
            dbContext.Transports.Update(transportEntity);
        }
        else
        {
            dbContext.Transports.Add(transportEntity);
        }

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

    public async Task<IList<TransportEntity>> GetTransportsByDateAsync(DateOnly date)
    {
        return await dbContext.Transports.Where(transport => transport.TransportedDate == date).ToListAsync();
    }
}

