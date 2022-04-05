using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AzureFunctions.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AzureFunctions.Startup))]
namespace AzureFunctions
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<TransportRepository>();
            builder.Services.AddSingleton<AggregationService>();
            builder.Services.AddSingleton<ReportRepository>();
        }
    }
}