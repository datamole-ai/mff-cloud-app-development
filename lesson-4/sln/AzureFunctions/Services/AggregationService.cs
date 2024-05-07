using AzureFunctions.Models;

using Microsoft.Extensions.Logging;

using Observability.Conventions;

namespace AzureFunctions.Services;

public class AggregationService(TransportsRepository transportsRepository, StatisticsCacheRepository statisticsCacheRepository, ILogger<AggregationService> logger)
{
    public async Task<DayStatistics> GetDayStatisticsAsync(DateOnly date, CancellationToken cancellationToken)
    {
        var activity = Instrumentation.ActivitySource.StartActivity();
        
        logger.LogInformation("Activity {activityId} started", activity?.SpanId);
        
        var statistics = await statisticsCacheRepository.FindStatisticsAsync(date, cancellationToken);    
        
        if (statistics is not null)
        {
            activity?.AddTag(SemanticConventions.AttributeCacheHit, SemanticConventions.AttributeCacheHitValues.True);
            return statistics;
        }
        
        activity?.AddTag(SemanticConventions.AttributeCacheHit, SemanticConventions.AttributeCacheHitValues.False);
        
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

        return computedStatistics ?? new DayStatistics(date, 0, double.NaN);
    }
}
