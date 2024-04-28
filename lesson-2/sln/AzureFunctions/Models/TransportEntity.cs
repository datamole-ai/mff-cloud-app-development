using Azure;
using Azure.Data.Tables;

namespace AzureFunctions.Models;

public record TransportEntity(
    string PartitionKey,
    string RowKey,
    DateTimeOffset? Timestamp,
    ETag ETag,
    string FacilityId,
    string ParcelId,
    DateTimeOffset TransportedAt,
    string LocationFrom,
    string LocationTo,
    long TimeSpentSeconds,
    string DeviceId): ITableEntity
{

    public static TransportEntity FromTransport(Transport transport)
    {
        var date = DateOnly.FromDateTime(dateTime: transport.TransportedAt.UtcDateTime);

        return new(
            PartitionKey: GeneratePartitionKey(date, transport.FacilityId),
            RowKey: transport.ParcelId,
            ETag: default,
            Timestamp: default,
            FacilityId: transport.FacilityId,
            ParcelId: transport.ParcelId,
            TransportedAt: transport.TransportedAt,
            LocationFrom: transport.LocationFrom,
            LocationTo: transport.LocationTo,
            TimeSpentSeconds: transport.TransportDurationSec,
            DeviceId: transport.DeviceId);
    }

    public DateTimeOffset? Timestamp { get; set; } = Timestamp;
    public string PartitionKey { get; set; } = PartitionKey;
    public string RowKey { get; set; } = RowKey;
    public ETag ETag { get; set; } = ETag;
    
    public static string GeneratePartitionKey(DateOnly date, string facilityId) => $"{date:yyyy-MM-dd}_{facilityId}";
    
    public TransportEntity() : this(default!, default!, default!, default!, default!, default!, default, default!, default!, default, default!)
    {
        
    }
}
