using System.Diagnostics;
using System.Diagnostics.Metrics;

using Azure.Messaging.EventHubs;

using PartitionContext = AzureFunctions.Models.PartitionContext;

namespace AzureFunctions;

public static class Instrumentation
{
    internal const string ActivitySourceName = "Platform.AzureFunctions";
    internal const string MeterName = "Platform.AzureFunctions";
    
    private static Meter Meter { get; } = new(MeterName);
    public static ActivitySource ActivitySource { get; } = new(ActivitySourceName);
    public static Counter<long> ProcessedEventsCounter { get; } = Meter.CreateCounter<long>(MetricNameProcessedEventsCount, description: "Number of processed events.");
    public static Counter<long> EventConsumerInvocationsCounter { get; } = Meter.CreateCounter<long>(MetricNameInvocationsCount, description: "Number of invocations of the consumer function.");
    public static Histogram<double> BatchProcessingDurationHistogram { get; } = Meter.CreateHistogram<double>(MetricNameBatchProcessingDuration, description: "Duration of event processing.", unit: "s");
    
    public static void RecordInvocationMetrics(EventData[] events, PartitionContext partitionContext, TimeSpan duration)
    {
        var labels = new KeyValuePair<string, object?>[]
        {
            new("partition_id", partitionContext.PartitionId),
        };
        
        ProcessedEventsCounter.Add(events.Length, labels);
        EventConsumerInvocationsCounter.Add(1, labels);
        BatchProcessingDurationHistogram.Record(duration.TotalSeconds, labels);
    }
    
    
    private const string MetricNameProcessedEventsCount = "mffiot.processed_events_count";
    private const string MetricNameInvocationsCount = "mffiot.invocations_count";
    private const string MetricNameBatchProcessingDuration = "mffiot.batch_processing_duration";
    
    public const string AttributeBatchSize = "mff_iot.batch_size";
    public const string AttributePartitionId = "mff_iot.partition_id";
    public const string AttributeEventHubNamespace = "mff_iot.eh_namespace";
    public const string AttributeFirstSequenceNumber = "mff_iot.first_seq_number";
    public const string AttributeLastSequenceNumber = "mff_iot.last_seq_number";
}
