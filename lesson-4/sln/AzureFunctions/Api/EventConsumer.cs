using System.Diagnostics;
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
    public async Task Run(
        [EventHubTrigger("transports", Connection = "EventHubConnectionString")] EventData[] events, PartitionContext partitionContext, CancellationToken cancellationToken)
    {
        using var activity = Instrumentation.ActivitySource.StartActivity("Process Transports Batch");
        
        activity?.AddTag("mff_iot.batch_size", events.Length);
        activity?.AddTag("mff_iot.partition_id", partitionContext.PartitionId);
        activity?.AddTag("mff_iot.eh_namespace", partitionContext.FullyQualifiedNamespace);
        activity?.AddTag("mff_iot.first_seq_number", events[0].SequenceNumber);
        activity?.AddTag("mff_iot.last_seq_number", events[^1].SequenceNumber);
        
        var startTime = Stopwatch.GetTimestamp();
        
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

        var duration = Stopwatch.GetElapsedTime(startTime);
        
        Instrumentation.RecordInvocationMetrics(events, partitionContext, duration);
        
        logger.LogInformation("Batch of {length} events processed.", events.Length);
    }
}
