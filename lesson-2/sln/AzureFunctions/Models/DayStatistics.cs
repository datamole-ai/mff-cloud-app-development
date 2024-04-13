using System;
  
namespace AzureFunctions.Models;

public record DayStatistics(
    DateTimeOffset Day,
    int TotalTransportedCount,
    double AverageTransportationTime);
