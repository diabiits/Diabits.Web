using Diabits.Web.DTOs;
using Diabits.Web.Infrastructure.Api;

using MudBlazor.Extensions;

namespace Diabits.Web.Services.HealthData;

public class HealthDataService
{
    private readonly ApiClient _apiClient;
    public HealthDataService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<HealthDataResponse> GetHealthDataAsync(DateTime startDate, DateTime endDate)
    {
        var start = startDate.ToString("yyyy-MM-ddTHH:mm:ss");
        var end = endDate.ToString("yyyy-MM-ddTHH:mm:ss");
        var result = await _apiClient.GetAsync<HealthDataResponse>($"HealthData?startDate={start}&endDate={end}");

        if (!result.IsSuccess)
        {
            throw new HttpRequestException(result.Error ?? "Something went wrong while getting data");
        }

        if (result.Data == null ) //TODO Refactor
        { 
            throw new HttpRequestException(result.Error ?? "No data found"); 
        }   

        return result.Data;
    }
}
