using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AzureFunctions.Models;
using System;
using System.Collections.Generic;

namespace AzureFunctions.Services
{
    public class AggregationService
    {
        private class WarehouseRunningStats
        {
            public WarehouseRunningStats(string warehouseId)
            {
                WarehouseId = warehouseId;
            }
            public string WarehouseId { get; }

            public double SumTime { get; set; }
            public int TotalTransported { get; set; }
        }

        private readonly TransportRepository _repository;
        private readonly ILogger<AggregationService> _logger;

        public AggregationService(
            TransportRepository repository,
            ILogger<AggregationService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<WarehouseDayStatistics>> AggregateStatisticsForDayAsync(DateTimeOffset dateTimeOffset)
        {
            var warehouses = new List<WarehouseDayStatistics>();

            WarehouseRunningStats? currentWarehouse = null;

            // the order is based on the partition key and then the row key
            // see https://docs.microsoft.com/en-us/azure/storage/tables/table-storage-design-for-query#sorting-data-in-the-table-service
            await foreach (var item in _repository.GetTransportsInADayAsync(dateTimeOffset))
            {
                if (currentWarehouse is null)
                {
                    currentWarehouse = new WarehouseRunningStats(item.WarehouseId);
                }

                if (currentWarehouse.WarehouseId != item.WarehouseId)
                {
                    var warehouseStats = new WarehouseDayStatistics(
                        currentWarehouse.WarehouseId,
                        dateTimeOffset.Date,
                        currentWarehouse.TotalTransported,
                        currentWarehouse.SumTime / currentWarehouse.TotalTransported
                    );

                    warehouses.Add(warehouseStats);
                    currentWarehouse = new WarehouseRunningStats(item.WarehouseId);
                }
                else
                {
                    currentWarehouse.SumTime += item.TransportDurationSec!.Value;
                    currentWarehouse.TotalTransported++;
                }
            }

            return warehouses;
        }
    }
}
