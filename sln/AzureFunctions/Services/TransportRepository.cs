using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AzureFunctions.Models;
using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace AzureFunctions.Services
{
    public class TransportRepository
    {
        private readonly ILogger<TransportRepository> _logger;
        private readonly CloudTable _table;

        private readonly string _tableName;
        public TransportRepository(ILogger<TransportRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // TODO env props 
            var storageConnectionString = Environment.GetEnvironmentVariable("StorageAccountConnectionString");
            _tableName = "transports";

            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            _table = tableClient.GetTableReference(_tableName);
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
                await _table.ExecuteAsync(TableOperation.InsertOrMerge(entity));
            }
            catch (Exception e)
            {
                _logger.LogError($"Unexpected error while saving to {_tableName}: {e}");
                throw;
            }
        }

        public async IAsyncEnumerable<Transport> GetTransportsInADayAsync(DateTimeOffset datetime)
        {
            var partitionKey = datetime.ToString(TransportEntity.PartitionKeyFormat);
            var filterPk = TableQuery.GenerateFilterCondition(
                nameof(TransportEntity.PartitionKey),
                QueryComparisons.Equal, partitionKey);

            var query = new TableQuery<TransportEntity>().Where(filterPk);
            TableContinuationToken? continuationToken = null;
            do
            {
                var page = await _table.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = page.ContinuationToken;

                foreach (var entity in page.Results)
                {
                    var model = TransportEntity.ToModel(entity);
                    yield return model;
                }
            }
            while (continuationToken != null);
        }
    }
}
