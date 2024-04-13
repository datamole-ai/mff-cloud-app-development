using System;

namespace AzureFunctions.Models;

public record Transport(
    string ParcelId,
    DateTimeOffset TransportedAt,
    string LocationFrom,
    string LocationTo,
    long TimeSpentSeconds,
    string FacilityId,
    string DeviceId);
