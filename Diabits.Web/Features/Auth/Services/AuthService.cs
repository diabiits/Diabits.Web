using Diabits.Web.Infrastructure.Api;

namespace Diabits.Web.Features.Auth.Services;

/// <summary>
/// Handles authentication business logic and state management.
/// </summary>
public sealed class AuthService
{
    private readonly ApiClient _apiClient;
    private readonly TokenStorage _tokens;
    private readonly JwtAuthStateProvider _authProvider;

    public AuthService(ApiClient apiClient, TokenStorage tokens, JwtAuthStateProvider authStateProvider)
    {
        _apiClient = apiClient;
        _tokens = tokens;
        _authProvider = authStateProvider;
    }

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        var result = await _apiClient.PostAsync<AuthResponse>("Auth/login", new LoginRequest(username, password));

        if (!result.IsSuccess)
            return AuthResult.Fail(result.Error ?? "Login failed");

        await _tokens.SaveAsync(new AuthSession(result.Data!.AccessToken));
        _authProvider.NotifyAuthenticationStateChanged();

        return AuthResult.Success();
    }

    public async Task LogoutAsync()
    {
        await _tokens.ClearAsync();
        _authProvider.NotifyAuthenticationStateChanged();
        // TODO: Call backend logout endpoint when implementing refresh tokens
    }

    public async Task<AuthResult> UpdateCredentialsAsync(string currentPassword, string? newUsername = null, string? newPassword = null)
    {
        var result = await _apiClient.PutAsync<AuthResponse>("Auth/UpdateCredentials", new UpdateAccountRequest(currentPassword, newUsername, newPassword));

        if (!result.IsSuccess)
            return AuthResult.Fail(result.Error ?? "Update failed");

        await _tokens.SaveAsync(new AuthSession(result.Data!.AccessToken));
        _authProvider.NotifyAuthenticationStateChanged();

        return AuthResult.Success();
    }

    //TODO Implement dialog that asks user to log back in when access token expires
    public async Task<bool> IsAuthenticatedAsync()
    {
        var state = await _authProvider.GetAuthenticationStateAsync();
        return state.User.Identity?.IsAuthenticated ?? false;
    }

    private record LoginRequest(string Username, string Password);
    private record UpdateAccountRequest(string CurrentPassword, string? NewUsername, string? NewPassword);
    private record AuthResponse(string AccessToken);

    public record AuthResult(bool Ok, string? Error)
    {
        public static AuthResult Success() => new(true, null);
        public static AuthResult Fail(string error) => new(false, error);
    }
}