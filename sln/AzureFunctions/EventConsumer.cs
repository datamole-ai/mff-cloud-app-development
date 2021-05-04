using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AzureFunctions.Models;
using System;
using AzureFunctions.Services;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AzureFunctions
{
    public class EventConsumer
    {
        private readonly TransportRepository _transportRepository;
        private readonly ILogger<EventConsumer> _logger;
        public EventConsumer(
            TransportRepository storeTransportService,
            ILogger<EventConsumer> logger
            )
        {
            if (storeTransportService is null)
            {
                throw new ArgumentNullException(nameof(storeTransportService));
            }

            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _transportRepository = storeTransportService;
            _logger = logger;
        }

        [Function("EventConsumer")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            Transport record;
            try
            {
                var str = await req.ReadAsStringAsync();
                if (string.IsNullOrEmpty(str))
                {
                    _logger.LogError("empty body");
                    return req.CreateResponse(HttpStatusCode.BadRequest);
                }
                record = JsonSerializer.Deserialize<Transport>(str)!;
            }
            catch (AggregateException e) when (e.InnerExceptions[0] is JsonException)
            {
                _logger.LogError("Invalid request body: {error}", e.InnerExceptions[0].Message);
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (ValidateRecord(record))
            {
                _logger.LogError("Invalid record properties");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            await _transportRepository.StoreAsync(record);

            _logger.LogInformation("Request successfully processed");
            return req.CreateResponse(HttpStatusCode.Accepted);
        }

        private static bool ValidateRecord(Transport record) =>
            record is null
            || string.IsNullOrEmpty(record.LocationFrom)
            || string.IsNullOrEmpty(record.LocationTo)
            || string.IsNullOrEmpty(record.ObjectId)
            || string.IsNullOrEmpty(record.WarehouseId)
            || !Regex.IsMatch(record.WarehouseId, "[A-Za-z0-9-_]{12,64}")
            || !record.TimeSpentSeconds.HasValue;
    }
}
