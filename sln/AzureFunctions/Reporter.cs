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
        private readonly ReportRepository _reportRepository;
        private readonly ILogger<Reporter> _logger;
        private const string _dayParamName = "day";
        private const string _warehouseParamName = "warehouse";

        public Reporter(
            ReportRepository reportRepository,
            ILogger<Reporter> logger)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [Function("Reporter")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            var query = req.Url.Query;
            if (string.IsNullOrEmpty(query))
            {
                _logger.LogError("Uri does not contain any query");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            var queryParams = HttpUtility.ParseQueryString(req.Url.Query);
            var days = queryParams.GetValues(_dayParamName);
            if (days is null || days.Length != 1)
            {
                _logger.LogError("Expected number of {dayName} is 1", _dayParamName);
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

            var warehouses = queryParams.GetValues(_warehouseParamName);
            if (warehouses is null || warehouses.Length != 1)
            {
                _logger.LogError("Expected number of {warehouseName} is 1", _warehouseParamName);
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
            var warehouse = warehouses[0];

            _logger.LogInformation("Querying report for Day {day} and Warehouse: {warehouse}",
                day.ToString("s"),
                warehouse);
            try
            {
                var stats = await _reportRepository.GetWarehouseDayStatisticsAsync(warehouse, day);
                if (stats is null)
                {
                    return req.CreateResponse(HttpStatusCode.NoContent);
                }
                else
                {
                    var response = req.CreateResponse(HttpStatusCode.OK);
                    await response.WriteAsJsonAsync(stats);
                    return response;
                }
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError("Error while fetching the statistics. Error {error}", ioe);
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
        }
    }
}
