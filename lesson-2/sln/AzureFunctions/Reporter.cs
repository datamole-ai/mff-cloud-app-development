using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AzureFunctions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        [FunctionName("Reporter")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            var query = req.Query.ToString();
            if (string.IsNullOrEmpty(query))
            {
                _logger.LogError("Uri does not contain any query");
                return new BadRequestResult();
            }

            var queryParams = HttpUtility.ParseQueryString(query);
            var days = queryParams.GetValues(_dayParamName);
            if (days is null || days.Length != 1)
            {
                _logger.LogError("Expected number of {dayName} is 1", _dayParamName);
                return new BadRequestResult();
            }

            var dayText = days[0];
            if (!DateTimeOffset.TryParseExact(dayText,
                "yyyyMMdd",
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out var day))
            {
                _logger.LogError("Could not parse day {dayText}", dayText);
                return new BadRequestResult();
            }

            var warehouses = queryParams.GetValues(_warehouseParamName);
            if (warehouses is null || warehouses.Length != 1)
            {
                _logger.LogError("Expected number of {warehouseName} is 1", _warehouseParamName);
                return new BadRequestResult();
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
                    return new NoContentResult();
                }
                else
                {
                    return new OkObjectResult(new StringContent(JsonConvert.SerializeObject(stats), Encoding.UTF8, "application/json"));
                }
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogError("Error while fetching the statistics. Error {error}", ioe);
                return new BadRequestResult();
            }
        }
    }
}
