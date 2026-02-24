using Diabits.Web.DTOs;
using Diabits.Web.Infrastructure.Api;

using static Diabits.Web.Components.Dashboard.DailyGlucoseTab;
using static Diabits.Web.Components.Dashboard.TimelineTab;

namespace Diabits.Web.Services.Dashboard;

public class DashboardService
{
    private readonly ApiClient _apiClient;

    public DashboardService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<TimelineChartResponse> GetTimelineAsync(DateTime date)
    {
        //TODO formatting date?
        var dateStr = date.ToString("yyyy-MM-ddTHH:mm:ss");
        var result = await _apiClient.GetAsync<TimelineChartResponse>($"Dashboard/timeline?date={dateStr}");

        if (!result.IsSuccess)
        {
            throw new HttpRequestException(result.Error ?? "Failed to load data");
        }

        if (result.Data == null)
        {
            throw new HttpRequestException("No data returned");
        }

        return result.Data;
    }

    public async Task<DailyGlucoseResponse> GetDailyGlucoseAsync(DateTime date)
    {
        var dateStr = date.ToString("yyyy-MM-dd");
        var result = await _apiClient.GetAsync<DailyGlucoseResponse>($"Dashboard/glucose/daily?date={dateStr}");

        if (!result.IsSuccess)
        {
            throw new HttpRequestException(result.Error ?? "Failed to load data");
        }

        if (result.Data == null)
        {
            throw new HttpRequestException("No data returned");
        }

        return result.Data;
    }
}

