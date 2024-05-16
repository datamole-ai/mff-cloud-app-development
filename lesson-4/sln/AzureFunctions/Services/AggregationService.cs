using System.Diagnostics;

using AzureFunctions.Models;

using Microsoft.Extensions.Logging;

using Observability.Conventions;

namespace AzureFunctions.Services;

public class AggregationService(TransportsRepository transportsRepository, StatisticsCacheRepository statisticsCacheRepository, ILogger<AggregationService> logger)
{
    public async Task<DayStatistics?> GetDayStatisticsAsync(DateOnly date, CancellationToken cancellationToken)
    {
        using var activity = Instrumentation.ActivitySource.StartActivity();
        
        logger.LogInformation("Activity {activityId} started", activity?.SpanId);

        var statistics = await TryGetStatisticsFromCacheAsync(date, cancellationToken);   
        
        if (statistics is not null)
        {
            return statistics;
        }

        var computedStatistics = await ComputeStatisticsAsync(date, cancellationToken);

        if (computedStatistics is not null)
        {
            await statisticsCacheRepository.SaveStatisticsToCacheAsync(computedStatistics, cancellationToken);
        }

        return computedStatistics;
    }
    
    private async Task<DayStatistics?> TryGetStatisticsFromCacheAsync(DateOnly date, CancellationToken cancellationToken)
    {
        using var activity = Instrumentation.ActivitySource.StartActivity();
        var statistics = await statisticsCacheRepository.FindStatisticsAsync(date, cancellationToken);
        
        if (statistics is not null)
        {
            activity?.AddTag(SemanticConventions.AttributeCacheHit, SemanticConventions.AttributeCacheHitValues.True);
            return statistics;
        }
        
        activity?.AddTag(SemanticConventions.AttributeCacheHit, SemanticConventions.AttributeCacheHitValues.False);

        return null;
    }
    
    private async Task<DayStatistics?> ComputeStatisticsAsync(DateOnly date, CancellationToken cancellationToken)
    {
        using var activity = Instrumentation.ActivitySource.StartActivity();
        
        var transports = transportsRepository.GetTransportsByDateAsync(date);

        var totalTransportedCount = 0;
        var totalTimeSpentSeconds = (long) 0;
        
        await foreach (var transport in transports)
        {
            totalTransportedCount++;
            totalTimeSpentSeconds += transport.TimeSpentSeconds;
        }

        return totalTransportedCount == 0 ? null : 
            new DayStatistics(date, totalTransportedCount, totalTimeSpentSeconds / (double) totalTransportedCount);
    }
}
