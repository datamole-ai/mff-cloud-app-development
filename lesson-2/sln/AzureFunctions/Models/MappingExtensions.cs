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
}
