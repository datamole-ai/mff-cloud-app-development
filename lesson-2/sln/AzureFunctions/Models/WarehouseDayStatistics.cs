using System;
using System.Text.Json.Serialization;

namespace AzureFunctions.Models
{
    public class WarehouseDayStatistics
    {
        public WarehouseDayStatistics(
            string warehouseId,
            DateTimeOffset day,
            int totalTransported,
            double averageTimeOfTransportation)
        {
            if (string.IsNullOrEmpty(warehouseId))
            {
                throw new ArgumentException($"'{nameof(warehouseId)}' cannot be null or empty.", nameof(warehouseId));
            }

            WarehouseId = warehouseId;
            Day = day;
            TotalTransported = totalTransported;
            AverageTimeOfTransportation = averageTimeOfTransportation;
        }
        public WarehouseDayStatistics()
        {

        }

        [JsonPropertyName("warehouseId")]
        public string WarehouseId { get; set; } = null!;


        [JsonPropertyName("day")]
        public DateTimeOffset Day { get; set; }

        [JsonPropertyName("totalTransported")]
        public int TotalTransported { get; set; }

        [JsonPropertyName("avgTimeOfTransportation")]
        public double AverageTimeOfTransportation { get; set; }
    }
}
