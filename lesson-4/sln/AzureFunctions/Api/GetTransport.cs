using AzureFunctions.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunctions.Api;

public class GetTransport(TransportsRepository transportsRepository)
{
    [Function("GetTransport")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req, CancellationToken cancellationToken)
    {
        req.Query.TryGetValue("date", out var dateQuery);
        req.Query.TryGetValue("facilityId", out var facilityIdQuery);
        req.Query.TryGetValue("parcelId", out var parcelIdQuery);

        if (!DateOnly.TryParseExact(dateQuery, "yyyy-MM-dd", out var date) || string.IsNullOrEmpty(facilityIdQuery) || string.IsNullOrEmpty(parcelIdQuery))
        {
            return new BadRequestResult();
        }

        return await transportsRepository.FindTransportAsync(date, facilityIdQuery!, parcelIdQuery!, cancellationToken) switch
        {
            { } transport => new OkObjectResult(transport),
            _ => new NotFoundResult()
        };
    }
}
