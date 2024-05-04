using Azure.Messaging.EventHubs;

using AzureFunctions.Models;
using AzureFunctions.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;


namespace AzureFunctions.Api;

public class EventConsumer(TransportsRepository transportsRepository, ILogger<EventConsumer> logger)
{
    [Function("EventConsumer")]
    public async Task<IActionResult> Run(
        [EventHubTrigger("transports", Connection = "EventhubNamespaceConnectionString")] EventData[] events, CancellationToken cancellationToken)
    {
        if (transport is null)
        {
            return new BadRequestResult();
        }

        await transportsRepository.SaveTransportAsync(transport, cancellationToken);

        logger.LogInformation("Transport {transport} saved.", transport);

        return new CreatedResult(null as string, transport);
    }
}
