using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AzureFunctions.Models;
using System;
using System.Collections.Generic;
using Azure.Data.Tables;

namespace AzureFunctions.Services
{
    public class TransportRepository
    {
        private readonly ILogger<TransportRepository> _logger;
        private readonly TableClient _tableClient;

        private readonly string _tableName;
        public TransportRepository(ILogger<TransportRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var storageConnectionString = Environment.GetEnvironmentVariable("StorageAccountConnectionString");
            _tableName = "transports";

            _tableClient = new TableClient(storageConnectionString, _tableName);
        }

        public async Task StoreAsync(Transport transport)
        {
            if (transport is null)
            {
                throw new ArgumentNullException(nameof(transport));
            }
            var transportId = Guid.NewGuid().ToString();
            var entity = TransportEntity.FromTransport(transportId, transport);
            try
            {
                await _tableClient.AddEntityAsync(entity);
            }
            catch (Exception e)
            {
                _logger.LogError($"Unexpected error while saving to {_tableName}: {e}");
                throw;
            }
        }

        public async IAsyncEnumerable<Transport> GetTransportsInADayAsync(DateTimeOffset datetime)
        {
            var queryResponse = _tableClient.QueryAsync<TransportEntity>(e =>
                e.PartitionKey == datetime.ToString(TransportEntity.PartitionKeyFormat));

            await foreach(var item in queryResponse)
            {
                var model = TransportEntity.ToModel(item);
                yield return model;

            }
        }
    }
}
