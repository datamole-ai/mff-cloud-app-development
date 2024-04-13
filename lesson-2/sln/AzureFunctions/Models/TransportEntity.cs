using System;

namespace AzureFunctions.Models;

public record TransportEntity(
    DateOnly TransportedDate,
    string FacilityId,
    string ParcelId,
    DateTimeOffset TransportedAt,
    string LocationFrom,
    string LocationTo,
    long TimeSpentSeconds,
    string DeviceId)
{
    public static TransportEntity FromTransport(Transport transport) =>
        new(TransportedDate: DateOnly.FromDateTime(dateTime: transport.TransportedAt.UtcDateTime),
            FacilityId: transport.FacilityId,
            ParcelId: transport.ParcelId,
            TransportedAt: transport.TransportedAt,
            LocationFrom: transport.LocationFrom,
            LocationTo: transport.LocationTo,
            TimeSpentSeconds: transport.TimeSpentSeconds,
            DeviceId: transport.DeviceId);
}
