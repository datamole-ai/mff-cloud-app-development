using AzureFunctions.Models;

namespace AzureFunctions.Services;

public class AggregationService(TransportsRepository transportsRepository, StatisticsCacheRepository statisticsCacheRepository)
{
    public async Task<DayStatistics?> GetDayStatisticsAsync(DateOnly date, CancellationToken cancellationToken)
    {
        var statistics = await statisticsCacheRepository.FindStatisticsAsync(date, cancellationToken);    
        
        if (statistics is not null)
        {
            return statistics;
        }
        
        var transports = transportsRepository.GetTransportsByDateAsync(date);

        var totalTransportedCount = 0;
        var totalTimeSpentSeconds = (long) 0;
        
        await foreach (var transport in transports)
        {
            totalTransportedCount++;
            totalTimeSpentSeconds += transport.TimeSpentSeconds;
        }

        var computedStatistics = totalTransportedCount == 0 ? null : 
            new DayStatistics(date, totalTransportedCount, totalTimeSpentSeconds / (double) totalTransportedCount);

        if (computedStatistics is not null)
        {
            await statisticsCacheRepository.SaveStatisticsAsync(computedStatistics, cancellationToken);
        }

        return computedStatistics;
    }
}
