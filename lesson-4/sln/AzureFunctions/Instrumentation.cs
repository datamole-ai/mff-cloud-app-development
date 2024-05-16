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
    
    
    public const string MetricNameProcessedEventsCount = "mffiot.processed_events_count";
    public const string MetricNameInvocationsCount = "mffiot.invocations_count";
    public const string MetricNameBatchProcessingDuration = "mffiot.batch_processing_duration";
}
