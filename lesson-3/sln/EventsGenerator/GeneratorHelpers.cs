namespace EventsGenerator;

public static class GeneratorHelpers
{
    public static int ParcelCounter { get; private set; }
    public const int ParcelsPerDay = 10000;
    public const int Days = 10;
    
    public static int DaysLeft => Days - (ParcelCounter / ParcelsPerDay);
    
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
        var facilityCityIndex = Random.Shared.Next(0, FacilityLocations.Length - 1);
        var facilityLocationId = Random.Shared.Next(0, 10);
        var facility = $"{FacilityLocations[facilityCityIndex]}-{facilityLocationId}";
        
        return new(
            ParcelId: $"PRCL{ParcelCounter++}",
            TransportedAt: DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(Days-(ParcelCounter/ (double) ParcelsPerDay))),
            LocationFrom: $"IN-{Random.Shared.Next(0, 100)}",
            LocationTo: $"OUT-{Random.Shared.Next(0, 100)}",
            TransportDurationSec: Random.Shared.NextInt64(20, 1000),
            FacilityId: facility,
            DeviceId: $"DEV-{facilityCityIndex}-{facilityLocationId}-{Random.Shared.Next(0, 100)}");
    }
}
