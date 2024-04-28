using AzureFunctions.Models;

namespace AzureFunctions.Services;

public class AggregationService(TransportsRepository transportsRepository)
{
    public async Task<DayStatistics?> GetDayStatisticsAsync(DateOnly date)
    {
        var transports = transportsRepository.GetTransportsByDateAsync(date);

        var totalTransportedCount = 0;
        var totalTimeSpentSeconds = (long) 0;
        
        await foreach (var transport in transports)
        {
            totalTransportedCount++;
            totalTimeSpentSeconds += transport.TimeSpentSeconds;
        }

        return totalTransportedCount == 0 ? null : 
            new(date, totalTransportedCount, totalTimeSpentSeconds / (double) totalTransportedCount);
    }
}
