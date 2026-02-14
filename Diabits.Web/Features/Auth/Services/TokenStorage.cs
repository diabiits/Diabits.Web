using System.Text.Json;
using Microsoft.JSInterop;

namespace Diabits.Web.Features.Auth.Services;

/// <summary>
/// Manages JWT token persistence in browser's localStorage via JavaScript Interop.
/// Saves/loads/clears authentication tokens so they survive page refreshes.
/// </summary>
public class TokenStorage(IJSRuntime js)
{
    private const string Key = "diabits_auth";
    private readonly IJSRuntime _js = js;

    public async Task SaveAsync(AuthSession session)
    {
        var json = JsonSerializer.Serialize(session);
        await _js.InvokeVoidAsync("localStorage.setItem", Key, json);
    }

    public async Task<AuthSession?> LoadAsync()
    {
        var json = await _js.InvokeAsync<string?>("localStorage.getItem", Key);
        if (string.IsNullOrWhiteSpace(json)) return null;

        try { return JsonSerializer.Deserialize<AuthSession>(json); }
        catch { return null; }
    }

    public async Task ClearAsync() =>
        await _js.InvokeVoidAsync("localStorage.removeItem", Key);
}

public sealed record AuthSession(string AccessToken);

