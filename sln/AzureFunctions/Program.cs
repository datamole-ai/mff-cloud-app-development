using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AzureFunctions.Services;

namespace AzureFunctions
{
    // custom telemetry    
    // https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library?tabs=v2%2Ccmd#log-custom-telemetry-in-c-functions
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(
                    services =>
                    {
                        // see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0
                        services.AddSingleton<TransportRepository>();
                        services.AddSingleton<AggregationService>();
                        services.AddSingleton<ReportRepository>();
                    }
                )
                .Build();

            host.Run();
        }
    }

}