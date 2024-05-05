namespace EventsGenerator;

public static class GeneratorHelpers
{
    private static int ParcelCounter = 0;
    
    private static readonly string[] FacilityLocations =
    [
        "prague",
        "frankfurt",
        "berlin",
        "paris",
        "london",
        "madrid",
        "lisbon",
        "rome",
        "athens",
        "warsaw"
    ];

    public static Transport GenerateTransport()
    {
        var facilityCityIndex = Random.Shared.Next(0, GeneratorHelpers.FacilityLocations.Length - 1);
        var facilityLocationId = Random.Shared.Next(0, 10);
        var facility = $"{GeneratorHelpers.FacilityLocations[facilityCityIndex]}-{facilityLocationId}";
        
        return new(
            ParcelId: $"PRCL{Interlocked.Increment(ref ParcelCounter)}",
            TransportedAt: DateTimeOffset.UtcNow,
            LocationFrom: $"IN-{Random.Shared.Next(0, 100)}",
            LocationTo: $"OUT-{Random.Shared.Next(0, 100)}",
            TransportDurationSec: Random.Shared.NextInt64(20, 1000),
            FacilityId: facility,
            DeviceId: $"DEV-{facilityCityIndex}-{facilityLocationId}-{Random.Shared.Next(0, 100)}");
    }
}
