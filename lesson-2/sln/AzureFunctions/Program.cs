using AzureFunctions.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = new HostBuilder()
    .ConfigureFunctionsWebApplication();


hostBuilder.ConfigureServices((_, collection) =>
{
    collection.AddDbContext<TransportsDbContext>(optionsBuilder =>
    {
        optionsBuilder.UseInMemoryDatabase("Transports");
    });

    collection.AddScoped<TransportsRepository>();
    collection.AddScoped<AggregationService>();
});

hostBuilder
    .Build()
    .Run();
