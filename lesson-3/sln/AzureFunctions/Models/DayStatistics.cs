namespace AzureFunctions.Models;

public record DayStatistics(
    DateOnly Date,
    int TotalTransportedCount,
    double AverageTransportationTimeSeconds);
