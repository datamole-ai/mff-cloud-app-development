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
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, [FromBody] Transport? transport, CancellationToken cancellationToken)
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
