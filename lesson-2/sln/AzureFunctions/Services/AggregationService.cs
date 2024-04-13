using AzureFunctions.Models;

namespace AzureFunctions.Services;

public class AggregationService(TransportsRepository transportsRepository)
{
    public async Task<DayStatistics?> GetDayStatisticsAsync(DateOnly date)
    {
        var transports = await transportsRepository.GetTransportsByDateAsync(date);

        return transports.Count == 0 ? null : new(date, transports.Count, transports.Average(t => t.TimeSpentSeconds));
    }
}
