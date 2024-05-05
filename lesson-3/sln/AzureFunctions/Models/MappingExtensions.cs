namespace AzureFunctions.Models;

public static class MappingExtensions
{
    public static Transport ToDomainModel(this TransportEntity transport)
    {
        return new(
            FacilityId: transport.FacilityId,
            ParcelId: transport.ParcelId,
            TransportedAt: transport.TransportedAt,
            LocationFrom: transport.LocationFrom,
            LocationTo: transport.LocationTo,
            TransportDurationSec: transport.TimeSpentSeconds,
            DeviceId: transport.DeviceId);
    }
    
    public static DayStatistics ToDomainModel(this DayStatisticsCacheEntity statisticsCacheEntity)
    {
        return new(
            Date: statisticsCacheEntity.PartitionKey switch
            {
                { } partitionKey => DateOnly.ParseExact(partitionKey, "yyyy-MM-dd"),
                _ => throw new InvalidOperationException("Invalid partition key")
            },
            TotalTransportedCount: statisticsCacheEntity.TotalTransportedCount,
            AverageTransportationTimeSeconds: statisticsCacheEntity.AverageTransportationTimeSeconds);
    }
}
