using AzureFunctions.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = new HostBuilder()
    .ConfigureFunctionsWebApplication();


hostBuilder.ConfigureServices((context, collection) =>
{
    collection.AddDbContext<TransportsDbContext>(optionsBuilder =>
    {
        optionsBuilder.UseSqlServer(context.Configuration.GetValue<string>("TransportsDbConnectionString"));
    });

    collection.AddScoped<TransportsRepository>();
    collection.AddScoped<AggregationService>();
});

hostBuilder
    .Build()
    .Run();
