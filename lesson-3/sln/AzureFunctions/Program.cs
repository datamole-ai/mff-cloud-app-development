using AzureFunctions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = new HostBuilder()
    .ConfigureFunctionsWebApplication();

hostBuilder.ConfigureServices((_, services) =>
{
    services.AddScoped<TransportsRepository>();
    services.AddScoped<StatisticsCacheRepository>();
    services.AddScoped<AggregationService>();
});

hostBuilder
    .Build()
    .Run();
