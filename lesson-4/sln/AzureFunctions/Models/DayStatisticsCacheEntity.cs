using Azure;
using Azure.Data.Tables;

namespace AzureFunctions.Models;

public record DayStatisticsCacheEntity(string PartitionKey,
    string RowKey,
    DateTimeOffset? Timestamp,
    ETag ETag,
    int TotalTransportedCount,
    double AverageTransportationTimeSeconds
    ): ITableEntity
{
    public DateTimeOffset? Timestamp { get; set; } = Timestamp;
    public string PartitionKey { get; set; } = PartitionKey;
    public string RowKey { get; set; } = RowKey;
    public ETag ETag { get; set; } = ETag;
    
    public static DayStatisticsCacheEntity FromDomainModel(DayStatistics dayStatistics)
    {
        return new(
            PartitionKey: GeneratePartitionKey(dayStatistics.Date),
            RowKey: "statistics",
            ETag: default,
            Timestamp: default,
            TotalTransportedCount: dayStatistics.TotalTransportedCount,
            AverageTransportationTimeSeconds: dayStatistics.AverageTransportationTimeSeconds);
    }
    
    public static string GeneratePartitionKey(DateOnly date) => $"{date:yyyy-MM-dd}";
    
    public DayStatisticsCacheEntity() : this(default!, default!, default!, default!, default, default)
    {
        
    }
}
