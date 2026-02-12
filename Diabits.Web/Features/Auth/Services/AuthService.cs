using Microsoft.AspNetCore.Components.Authorization;

namespace Diabits.Web.Features.Auth.Services;

public sealed class AuthService(
    AuthApiClient apiClient,
    AuthenticationStateProvider authStateProvider)
{
    private JwtAuthStateProvider JwtProvider => (JwtAuthStateProvider)authStateProvider;

    public async Task<AuthApiClient.LoginResult> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var result = await apiClient.LoginAsync(username, password, ct);

        if (result.Ok)
        {
            // Notify Blazor's auth system that state has changed
            JwtProvider.NotifyAuthenticationStateChanged();
        }

        return result;
    }

    public async Task LogoutAsync()
    {
        await apiClient.LogoutAsync();
        JwtProvider.NotifyAuthenticationStateChanged();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var state = await authStateProvider.GetAuthenticationStateAsync();
        return state.User.Identity?.IsAuthenticated ?? false;
    }
}