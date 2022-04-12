using System;
using System.Collections.Generic;
using System.Globalization;
using Azure;
using Azure.Data.Tables;

namespace AzureFunctions.Models
{
    public class TransportEntity : ITableEntity
    {
        public static string PartitionKeyFormat { get; set; } = "yyyyMMdd";

        public TransportEntity()
        {

        }

        public TransportEntity(
            string transportId,
            string warehouseId,
            string objectId,
            DateTimeOffset transportedDateTimeOffset,
            double timeSpentSeconds,
            string locationFrom,
            string locationTo
            )
        {
            PartitionKey = transportedDateTimeOffset.ToString("yyyyMMdd");
            RowKey = warehouseId + ":" + transportId;

            TransportedDateTime = transportedDateTimeOffset.ToString("o");

            TransportId = transportId;
            WarehouseId = warehouseId;
            ObjectId = objectId;
            TimeSpentSeconds = timeSpentSeconds;
            LocationFrom = locationFrom;
            LocationTo = locationTo;
        }

        public string PartitionKey { get; set; } = null!;
        public string RowKey { get; set; } = null!;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string TransportId { get; set; } = null!;
        public string WarehouseId { get; set; } = null!;
        public string ObjectId { get; set; } = null!;
        public double TimeSpentSeconds { get; set; }
        public string LocationFrom { get; set; } = null!;

        public string LocationTo { get; set; } = null!;

        public DateTimeOffset TransportedDateTimeOffset() => DateTimeOffset.Parse(TransportedDateTime, CultureInfo.InvariantCulture);

        public string TransportedDateTime { get; set; } = null!;

        public static TransportEntity FromTransport(
            string transportId,
            Transport transport)
        {
            if (transport is null)
            {
                throw new ArgumentNullException(nameof(transport));
            }

            return new TransportEntity(
                transportId,
                transport.WarehouseId,
                transport.ObjectId,
                transport.TranportedDateTime,
                transport.TransportDurationSec!.Value,
                transport.LocationFrom,
                transport.LocationTo
            );
        }

        public static Transport ToModel(TransportEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new Transport
            {
                WarehouseId = entity.WarehouseId,
                ObjectId = entity.ObjectId,
                TranportedDateTime = entity.TransportedDateTimeOffset(),
                TransportDurationSec = entity.TimeSpentSeconds,
                LocationFrom = entity.LocationFrom,
                LocationTo = entity.LocationTo
            };
        }
    }
}
