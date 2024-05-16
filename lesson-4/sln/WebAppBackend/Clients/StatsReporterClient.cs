using System.Net;

using WebAppBackend.Models;

namespace WebAppBackend.Clients;

public class StatsReporterClient(IHttpClientFactory httpClientFactory)
{
    public const string HttpClientName = "Reporter";

    private readonly HttpClient httpClient = httpClientFactory.CreateClient(HttpClientName);
    
    public async Task<Transport?> FindTransportAsync(DateOnly date, string facilityId, string parcelId)
    {
        var response = await httpClient.GetAsync($"api/GetTransport?date={date:yyyy-MM-dd}&facilityId={facilityId}&parcelId={parcelId}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Transport>();
        }

        return null;
    }
    
    public async Task<DayStatistics?> GetDailyStatistics(DateOnly date)
    {
        var response = await httpClient.GetAsync($"api/GetDailyStatistics?date={date:yyyy-MM-dd}");

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<DayStatistics>();
    }
}
