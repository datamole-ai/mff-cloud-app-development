using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AzureFunctions.Models;
using System;

namespace AzureFunctions.Services
{
    public class AggregationService
    {
        private readonly TransportRepository _repository;
        private readonly ILogger<AggregationService> _logger;

        public AggregationService(
            TransportRepository repository,
            ILogger<AggregationService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DayStatistics?> AggregateStatisticsForDayAsync(DateTimeOffset dateTimeOffset)
        {
            var sumTime = 0d;
            var totalTransported = 0;
            await foreach (var item in _repository.GetTransportsInADayAsync(dateTimeOffset))
            {
                sumTime += item.TimeSpentSeconds!.Value;
                totalTransported++;
            }

            if (totalTransported == 0)
            {
                _logger.LogInformation("No records were found for a date {date}", dateTimeOffset.Date.ToString("s"));
                return null;
            }

            var avgTime = sumTime / totalTransported;
            return new DayStatistics(
                dateTimeOffset.Date,
                totalTransported,
                avgTime
            );
        }
    }
}
