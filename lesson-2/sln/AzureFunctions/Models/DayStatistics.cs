using System;
using System.Text.Json.Serialization;

namespace AzureFunctions.Models
{
    public class DayStatistics
    {
        public DayStatistics(
            DateTimeOffset day,
            int totalTransported,
            double averageTimeOfTransportation)
        {
            Day = day;
            TotalTransported = totalTransported;
            AvgDurationOfTransportation = averageTimeOfTransportation;
        }
        public DayStatistics()
        {

        }
        [JsonPropertyName("day")]
        public DateTimeOffset Day { get; set; }

        [JsonPropertyName("totalTransported")]
        public int TotalTransported { get; set; }

        [JsonPropertyName("avgDurationOfTransportation")]
        public double AvgDurationOfTransportation { get; set; }
    }
}
