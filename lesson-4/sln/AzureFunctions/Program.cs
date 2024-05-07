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
    .ConfigureFunctionsWebApplication();


hostBuilder.ConfigureLogging(loggingBuilder => 
    loggingBuilder.AddOpenTelemetry(options =>
    {
        options.AddOtlpExporter();
    })
);

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
            
            tracerProviderBuilder.AddSource("*");

            tracerProviderBuilder.AddConsoleExporter();
            tracerProviderBuilder.AddOtlpExporter(options => options.BatchExportProcessorOptions.ScheduledDelayMilliseconds = 1_000);
        });
});

hostBuilder
    .Build()
    .Run();
