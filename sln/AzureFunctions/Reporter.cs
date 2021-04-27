using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using AzureFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctions
{
    public class Reporter
    {
        private readonly AggregationService _aggregationService;
        private readonly ILogger<Reporter> _logger;
        private const string _keyParamName = "day";

        public Reporter(
            AggregationService aggregationService,
            ILogger<Reporter> logger)
        {
            _aggregationService = aggregationService ?? throw new ArgumentNullException(nameof(aggregationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [Function("Reporter")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var query = req.Url.Query;
            if (string.IsNullOrEmpty(query))
            {
                _logger.LogError("Uri does not contain any query");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var queryParams = HttpUtility.ParseQueryString(req.Url.Query);
            var days = queryParams.GetValues(_keyParamName);
            if (days is null || days.Length != 1)
            {
                _logger.LogError("Expected number of {dayName} is 1", _keyParamName);
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var dayText = days[0];
            if (!DateTimeOffset.TryParseExact(dayText,
                "yyyyMMdd",
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out var day))
            {
                _logger.LogError("Could not parse day {dayText}", dayText);
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }


            var statistics = await _aggregationService.AggregateStatisticsForDayAsync(day);

            if (statistics is null)
            {
                return req.CreateResponse(HttpStatusCode.NoContent);
            }
            else
            {
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(statistics);
                return response;
            }
        }
    }
}
