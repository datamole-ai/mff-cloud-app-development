using Microsoft.Extensions.DependencyInjection;
using AzureFunctions.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(AzureFunctions.Startup))]
namespace AzureFunctions
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            
            builder.GetContext().Configuration.Get<string>("");
            builder.Services.AddHttpClient();

            builder.Services.AddDbContext<TransportsDbContext>(optionsBuilder => optionsBuilder.UseSqlServer());

            builder.Services.AddSingleton<TransportsRepository>();
            builder.Services.AddSingleton<AggregationService>();
        }
    }
}
