using System.Net;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AzureFunctions.Models;
using System;
using AzureFunctions.Services;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureFunctions
{
    public class EventConsumer(TransportsRepository transportsRepository, ILogger<EventConsumer> logger)
    {
        [FunctionName("EventConsumer")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            Transport record;
            try
            {
                var str = await req.ReadAsStringAsync();
                if (string.IsNullOrEmpty(str))
                {
                    logger.LogError("empty body");

                    return new BadRequestResult();
                }
                record = JsonSerializer.Deserialize<Transport>(str)!;
            }
            catch (Exception e)
            {
                logger.LogError("Invalid request body: {error}", e);
                return new BadRequestResult();
            }

            // if (ValidateRecord(record))
            // {
            //     logger.LogError("Invalid record properties");
            //     return new BadRequestResult();
            // }

            await transportsRepository.SaveTransportAsync(record);

            logger.LogInformation("Request successfully processed");
            return new AcceptedResult();
        }
    }
}
