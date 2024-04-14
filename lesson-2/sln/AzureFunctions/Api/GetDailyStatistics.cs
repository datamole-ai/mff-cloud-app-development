using AzureFunctions.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunctions.Api;

public class GetDailyStatistics(AggregationService aggregationService)
{
    [Function("GetDailyStatistics")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        req.Query.TryGetValue("date", out var dateQuery);

        if (!DateOnly.TryParseExact(dateQuery, "yyyy-MM-dd", out var date))
        {
            return new BadRequestResult();
        }

        return await aggregationService.GetDayStatisticsAsync(date) switch
        {
            { } dayStatistics => new OkObjectResult(dayStatistics),
            _ => new NoContentResult()
        };
    }
}
