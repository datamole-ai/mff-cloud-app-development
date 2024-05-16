using AzureFunctions;
using AzureFunctions.Services;

using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var hostBuilder = new HostBuilder()
    .ConfigureFunctionsWebApplication((_, functionsWorkerApplicationBuilder) =>
    {
        functionsWorkerApplicationBuilder.UseMiddleware<ActivityTrackingMiddleware>();
    });


hostBuilder.ConfigureLogging(loggingBuilder => 
    loggingBuilder.AddOpenTelemetry(options =>
    {
        options.AddOtlpExporter();
        options.IncludeFormattedMessage = true;
    })
);

AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

hostBuilder.ConfigureServices((_, services) =>
{
    services.AddScoped<TransportsRepository>();
    services.AddScoped<StatisticsCacheRepository>();
    services.AddScoped<AggregationService>();

    services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .WithMetrics(meterProviderBuilder =>
        {
            meterProviderBuilder.AddMeter("*");
            meterProviderBuilder.AddConsoleExporter();
            meterProviderBuilder.AddOtlpExporter((_, readerOptions) =>
            {
                readerOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5_000;
            });
            
        })
        .WithTracing(tracerProviderBuilder =>
        {
            
            tracerProviderBuilder.AddSource("Azure.*", ActivityTrackingMiddleware.ActivitySourceName, Instrumentation.ActivitySourceName);
            tracerProviderBuilder.SetSampler(new AlwaysOnSampler());
            tracerProviderBuilder.AddConsoleExporter();
            tracerProviderBuilder.AddOtlpExporter(options => options.BatchExportProcessorOptions.ScheduledDelayMilliseconds = 1_000);
        });
});

hostBuilder
    .Build()
    .Run();
