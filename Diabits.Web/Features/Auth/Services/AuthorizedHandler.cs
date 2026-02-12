using System.Net.Http.Headers;

namespace Diabits.Web.Features.Auth.Services;

//TODO Unused currently
/// <summary>
/// Send request with access token
/// </summary>
public sealed class AuthorizedHandler : DelegatingHandler
{
    private readonly TokenStorage _storage;

    public AuthorizedHandler(TokenStorage storage) => _storage = storage;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var session = await _storage.LoadAsync();
        if (session is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        }

        return await base.SendAsync(request, ct);
    }
}
