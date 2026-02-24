using Diabits.Web.DTOs;
using Diabits.Web.Infrastructure.Api;

namespace Diabits.Web.Services.Dashboard;

public class DashboardService
{
    private readonly ApiClient _apiClient;

    public DashboardService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<TimelineResponse> GetTimelineAsync(DateTime date, int bucketMinutes = 10)
    {
        //TODO formatting date?
        var dateStr = date.ToString("yyyy-MM-ddTHH:mm:ss");
        var result = await _apiClient.GetAsync<TimelineResponse>($"Dashboard/timeline?date={dateStr}&bucketMinutes={bucketMinutes}");

        if (!result.IsSuccess)
        {
            throw new HttpRequestException(result.Error ?? "Failed to load timeline data");
        }

        if (result.Data == null)
        {
            throw new HttpRequestException("No timeline data returned");
        }

        return result.Data;
    }
}
