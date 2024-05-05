namespace EventsGenerator;

public record Transport(
    string ParcelId,
    DateTimeOffset TransportedAt,
    string LocationFrom,
    string LocationTo,
    long TransportDurationSec,
    string FacilityId,
    string DeviceId);
