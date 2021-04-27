using System;
using System.Text.Json.Serialization;

namespace AzureFunctions.Models
{
    public class Transport
    {
        [JsonPropertyName("locationFrom")]
        public string LocationFrom { get; set; } = null!;

        [JsonPropertyName("locationTo")]
        public string LocationTo { get; set; } = null!;

        [JsonPropertyName("timeSpentSec")]
        public double? TimeSpentSeconds { get; set; }

        [JsonPropertyName("objectId")]
        public string ObjectId { get; set; } = null!;

        [JsonPropertyName("transportedDateTime")]
        public DateTimeOffset TranportedDateTime { get; set; }
    }
}
