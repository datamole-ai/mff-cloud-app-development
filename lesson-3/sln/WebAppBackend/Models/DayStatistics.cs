namespace WebAppBackend.Models;

public record DayStatistics(
    DateOnly Date,
    int TotalTransportedCount,
    double AverageTransportationTimeSeconds);
