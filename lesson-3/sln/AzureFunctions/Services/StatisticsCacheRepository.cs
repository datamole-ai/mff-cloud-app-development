using Azure;
using Azure.Data.Tables;

using AzureFunctions.Models;

namespace AzureFunctions.Services;

public class StatisticsCacheRepository
{
    private readonly TableClient _tableClient;
    
    public StatisticsCacheRepository()
    {
        var storageConnectionString = Environment.GetEnvironmentVariable("TransportsStorageConnectionsString");

        _tableClient = new(storageConnectionString, "statisticsCache");
    }
    
    public async Task SaveStatisticsAsync(DayStatistics dayStatistics, CancellationToken cancellationToken)
    {
        var statisticsEntity = DayStatisticsCacheEntity.FromDomainModel(dayStatistics);

        await _tableClient.UpsertEntityAsync(statisticsEntity, TableUpdateMode.Replace, cancellationToken);
    }
    
    public async Task<DayStatistics?> FindStatisticsAsync(DateOnly date, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _tableClient.GetEntityAsync<DayStatisticsCacheEntity>(
                partitionKey: DayStatisticsCacheEntity.GeneratePartitionKey(date), 
                rowKey: "statistics",
                cancellationToken: cancellationToken);

            return entity.HasValue ? entity.Value.ToDomainModel() : null;
        } catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }
}
