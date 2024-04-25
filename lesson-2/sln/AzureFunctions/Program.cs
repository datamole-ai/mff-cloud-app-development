using AzureFunctions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = new HostBuilder()
    .ConfigureFunctionsWebApplication();


hostBuilder.ConfigureServices((context, collection) =>
{
    collection.AddScoped<TransportsRepository>();
    collection.AddScoped<AggregationService>();
});

hostBuilder
    .Build()
    .Run();
