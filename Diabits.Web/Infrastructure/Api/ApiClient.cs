using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Diabits.Web.Infrastructure.Api;

/// <summary>
/// Generic API client that handles all HTTP communication. Automatically includes JWT tokens via AuthorizationHandler.
/// </summary>
public class ApiClient
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ApiResult<T>> GetAsync<T>(string endpoint, CancellationToken ct = default)
    {
        try
        {
            var response = await _http.GetAsync(endpoint, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await GetErrorMessageAsync(response, ct);
                return ApiResult<T>.Failure(error);
            }

            var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct);
            return ApiResult<T>.Success(data);
        }
        catch (HttpRequestException)
        {
            return ApiResult<T>.Failure("Cannot connect to server");
        }
    }

    public async Task<ApiResult<TResponse>> PostAsync<TResponse>(string endpoint, object request, CancellationToken ct = default)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(endpoint, request, JsonOptions, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await GetErrorMessageAsync(response, ct);
                return ApiResult<TResponse>.Failure(error);
            }

            var data = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, ct);
            return ApiResult<TResponse>.Success(data);
        }
        catch (HttpRequestException)
        {
            return ApiResult<TResponse>.Failure("Cannot connect to server");
        }
    }

    public async Task<ApiResult<TResponse>> PutAsync<TResponse>(string endpoint, object request, CancellationToken ct = default)
    {
        try
        {
            var response = await _http.PutAsJsonAsync(endpoint, request, JsonOptions, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await GetErrorMessageAsync(response, ct);
                return ApiResult<TResponse>.Failure(error);
            }

            var data = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, ct);
            return ApiResult<TResponse>.Success(data);
        }
        catch (HttpRequestException)
        {
            return ApiResult<TResponse>.Failure("Cannot connect to server");
        }
    }

    /// <summary>
    /// DELETE request - returns success/failure result.
    /// </summary>
    public async Task<ApiResult<bool>> DeleteAsync(string endpoint, CancellationToken ct = default)
    {
        try
        {
            var response = await _http.DeleteAsync(endpoint, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await GetErrorMessageAsync(response, ct);
                return ApiResult<bool>.Failure(error);
            }

            return ApiResult<bool>.Success(true);
        }
        catch (HttpRequestException)
        {
            return ApiResult<bool>.Failure("Cannot connect to server");
        }
    }

    /// <summary>
    /// Helper to extract error message from a failed response.
    /// </summary>
    private async Task<string> GetErrorMessageAsync(HttpResponseMessage response, CancellationToken ct = default)
    {
        try
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            return string.IsNullOrWhiteSpace(error) ? "Request failed" : error;
        }
        catch
        {
            return "Request failed";
        }
    }
}

/// <summary>
/// Result wrapper for API calls using the Result pattern.
/// Avoids throwing exceptions for expected failures (e.g., validation errors, auth failures).
/// </summary>
public record ApiResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }

    private ApiResult() { }

    public static ApiResult<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data,
        Error = null
    };

    public static ApiResult<T> Failure(string error) => new()
    {
        IsSuccess = false,
        Data = default,
        Error = error
    };
}
