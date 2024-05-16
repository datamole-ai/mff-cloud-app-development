using System.Text.Json;

using Azure.Messaging.EventHubs;

using AzureFunctions.Models;
using AzureFunctions.Services;


using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;


namespace AzureFunctions.Api;

public class EventConsumer(TransportsRepository transportsRepository, ILogger<EventConsumer> logger)
{
    [Function("EventConsumer")]
    [ExponentialBackoffRetry(-1, "00:00:01", "00:05:00")]
    public async Task Run(
        [EventHubTrigger("transportsv2", Connection = "EventHubConnectionString")] EventData[] events, CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(events, new ParallelOptions()
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = 100
        }, async (eventData, token) =>
        {
            Transport? transport = null;

            try
            {
                transport = JsonSerializer.Deserialize<Transport>(eventData.Body.Span);
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to deserialize transport");
            }

            if (transport is not null)
            {
                await transportsRepository.SaveTransportAsync(transport, token);
            }
        });
        
        logger.LogInformation("Batch of {length} events processed.", events.Length);
    }
}
