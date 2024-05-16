using Azure;
using Azure.Data.Tables;

using AzureFunctions.Models;

namespace AzureFunctions.Services;

public class TransportsRepository
{
    private readonly TableClient _tableClient;
    
    public TransportsRepository()
    {
        var storageConnectionString = Environment.GetEnvironmentVariable("TransportsStorageConnectionsString");

        _tableClient = new(storageConnectionString, "transports");
    }

    public async Task SaveTransportAsync(Transport transport, CancellationToken cancellationToken)
    {
        using var activity = Instrumentation.ActivitySource.StartActivity();
        
        var transportEntity = TransportEntity.FromDomainModel(transport);

        await _tableClient.UpsertEntityAsync(transportEntity, TableUpdateMode.Replace, cancellationToken);
    }

    public async Task<Transport?> FindTransportAsync(DateOnly date, string facilityId, string parcelId, CancellationToken cancellationToken)
    {
        using var activity = Instrumentation.ActivitySource.StartActivity();
        
        try
        {
            var entity = await _tableClient.GetEntityAsync<TransportEntity>(partitionKey: TransportEntity.GeneratePartitionKey(date, facilityId), rowKey: parcelId,
                cancellationToken: cancellationToken);

            return entity.HasValue ? entity.Value.ToDomainModel() : null;
        } catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async IAsyncEnumerable<TransportEntity> GetTransportsByDateAsync(DateOnly date)
    {
        var transports= _tableClient.QueryAsync<TransportEntity>($"PartitionKey gt '{date:yyyy-MM-dd}_' and PartitionKey lt '{date:yyyy-MM-dd}|'");
        
        await foreach (var transport in transports)
        {
            yield return transport;
        }
    }
}

