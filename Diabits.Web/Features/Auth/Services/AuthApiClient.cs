using System.Net;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace Diabits.Web.Features.Auth.Services;

public sealed class AuthApiClient
{
    private readonly HttpClient _http;
    private readonly TokenStorage _storage;

    public AuthApiClient(HttpClient http, TokenStorage storage)
    {
        _http = http;
        _storage = storage;
    }

    public async Task<LoginResult> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("Auth/login", new LoginRequest(username, password), ct);

        if (res.StatusCode == HttpStatusCode.BadRequest)
        {
            var msg = await res.Content.ReadAsStringAsync(ct);
            return LoginResult.Fail(string.IsNullOrWhiteSpace(msg) ? "Invalid credentials." : msg);
        }

        if (!res.IsSuccessStatusCode)
            return LoginResult.Fail("Backend unavailable.");

        var dto = await res.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: ct);
        if (dto is null || string.IsNullOrWhiteSpace(dto.AccessToken))
            return LoginResult.Fail("Invalid response from backend.");

        await _storage.SaveAsync(new AuthSession(dto.AccessToken, dto.RefreshToken));
        return LoginResult.Success();
    }

    //TODO Add call to backend when implementing refresh token here
    public async Task LogoutAsync()
    {
        await _storage.ClearAsync();
    }

    public async Task<UpdateAccountResult> UpdateAccountAsync(
        string currentPassword,
        string? newUsername = null,
        string? newPassword = null,
        CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("User/updateAccount",
            new UpdateAccountRequest(currentPassword, newUsername, newPassword), ct);

        if (res.StatusCode == HttpStatusCode.BadRequest)
        {
            var msg = await res.Content.ReadAsStringAsync(ct);
            return UpdateAccountResult.Fail(string.IsNullOrWhiteSpace(msg)
                ? "Failed to update account. Check your input."
                : msg);
        }

        if (res.StatusCode == HttpStatusCode.Unauthorized)
        {
            return UpdateAccountResult.Fail("Current password is incorrect.");
        }

        if (!res.IsSuccessStatusCode)
            return UpdateAccountResult.Fail("Backend unavailable.");

        return UpdateAccountResult.Success();
    }

    //TODO Move
    private sealed record LoginRequest(string Username, string Password);
    private sealed record UpdateAccountRequest(string CurrentPassword, string? NewUsername, string? NewPassword);
    private sealed record AuthResponse(string AccessToken, string RefreshToken);

    public sealed record LoginResult(bool Ok, string? Error)
    {
        public static LoginResult Success() => new(true, null);
        public static LoginResult Fail(string error) => new(false, error);
    }
    public sealed record UpdateAccountResult(bool Ok, string? Error)
    {
        public static UpdateAccountResult Success() => new(true, null);
        public static UpdateAccountResult Fail(string error) => new(false, error);
    }
}