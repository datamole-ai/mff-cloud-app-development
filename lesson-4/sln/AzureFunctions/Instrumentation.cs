using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace AzureFunctions;

public static class Instrumentation
{
    internal const string ActivitySourceName = "Platform.AzureFunctions";
    internal const string MeterName = "Platform.AzureFunctions";
    private static Meter Meter { get; } = new(MeterName);
    
    public static ActivitySource ActivitySource { get; } = new(ActivitySourceName);

    public static Counter<long> ProcessedEventsCounter { get; } = Meter.CreateCounter<long>("mffiot.processed_events_count", description: "Number of processed events.");
    public static Counter<long> EventConsumerInvocationsCounter { get; } = Meter.CreateCounter<long>("mffiot.invocations_count", description: "Number of invocations of the consumer.");
}
