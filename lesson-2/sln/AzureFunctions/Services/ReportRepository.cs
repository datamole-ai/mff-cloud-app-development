using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureFunctions.Models;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Services
{
    public class ReportRepository
    {
        private const string _containerName = "reports";
        private readonly ILogger<ReportRepository> _logger;
        private readonly BlobContainerClient _blobContainerClient;

        public ReportRepository(ILogger<ReportRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var storageConnectionString = Environment.GetEnvironmentVariable("StorageAccountConnectionString");
            var blobServiceClient = new BlobServiceClient(storageConnectionString);
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        }

        public async Task StoreReportAsync(List<WarehouseDayStatistics> warehouseDayStatistics)
        {
            foreach (var warehouseStats in warehouseDayStatistics)
            {
                var blobName = GetBlobName(warehouseStats.WarehouseId, warehouseStats.Day);
                _logger.LogInformation("Uploading to blob {blobName}", blobName);
                var blobClient = _blobContainerClient.GetBlobClient(blobName);

                var json = JsonSerializer.Serialize(warehouseStats);

                using var stream = new MemoryStream();
                using var writer = new StreamWriter(stream);
                writer.Write(json);
                writer.Flush();
                stream.Position = 0;

                await blobClient.UploadAsync(stream, true);
            }
        }

        public async Task<WarehouseDayStatistics?> GetWarehouseDayStatisticsAsync(string warehouseId, DateTimeOffset day)
        {
            var blobName = GetBlobName(warehouseId, day);
            var client = _blobContainerClient.GetBlobClient(blobName);
            var exists = await client.ExistsAsync();
            if (!exists.Value)
            {
                return null;
            }

            var options = new BlobOpenReadOptions(false);
            var stream = await client.OpenReadAsync(options);

            try
            {
                return await JsonSerializer.DeserializeAsync<WarehouseDayStatistics>(stream);
            }
            catch (JsonException je)
            {
                throw new InvalidOperationException("Could not read document that is not warehouse stats", je);
            }
        }

        private static string GetBlobName(string warehouseId, DateTimeOffset day)
            => $"{warehouseId}/{day:yyyyMMdd}";

    }
}
