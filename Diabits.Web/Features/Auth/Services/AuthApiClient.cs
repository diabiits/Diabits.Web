using System.Net;
using System.Net.Http.Json;
using static System.Collections.Specialized.BitVector32;
using static System.Net.WebRequestMethods;

namespace Diabits.Web.Features.Auth.Services;

public sealed class AuthApiClient
{
    private readonly HttpClient _http;
    private readonly TokenStorage _tokens;

    public AuthApiClient(HttpClient http, TokenStorage storage)
    {
        _http = http;
        _tokens = storage;
    }

    public async Task<AuthResult> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("Auth/login", new LoginRequest(username, password), ct);

        if (res.StatusCode == HttpStatusCode.BadRequest)
        {
            var msg = await res.Content.ReadAsStringAsync(ct);
            return AuthResult.Fail(string.IsNullOrWhiteSpace(msg) ? "Invalid credentials." : msg);
        }

        if (!res.IsSuccessStatusCode)
            return AuthResult.Fail("Backend unavailable.");

        var dto = await res.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: ct);
        if (dto is null || string.IsNullOrWhiteSpace(dto.AccessToken))
            return AuthResult.Fail("Invalid response from backend.");

        await _tokens.SaveAsync(new AuthSession(dto.AccessToken, dto.RefreshToken));
        return AuthResult.Success();
    }

    //TODO Add call to backend when implementing refresh token here
    public async Task LogoutAsync()
    {
        await _tokens.ClearAsync();
    }

    public async Task<AuthResult> UpdateAccountAsync(
        string currentPassword,
        string? newUsername = null,
        string? newPassword = null,
        CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("User/updateAccount",
            new UpdateAccountRequest(currentPassword, newUsername, newPassword), ct);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return AuthResult.Fail(error);
        }
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result?.AccessToken == null)
            return AuthResult.Fail("Invalid response from server");

        if (!response.IsSuccessStatusCode)
            return AuthResult.Fail("Backend unavailable.");

        await _tokens.SaveAsync(new AuthSession(result.AccessToken, result.RefreshToken));

        return AuthResult.Success();
    }

    //TODO Move and refactor
    private record LoginRequest(string Username, string Password);
    private record UpdateAccountRequest(string CurrentPassword, string? NewUsername, string? NewPassword);
    private record AuthResponse(string AccessToken, string RefreshToken);

    public record AuthResult(bool Ok, string? Error)
    {
        public static AuthResult Success() => new(true, null);
        public static AuthResult Fail(string error) => new(false, error);
    }
}