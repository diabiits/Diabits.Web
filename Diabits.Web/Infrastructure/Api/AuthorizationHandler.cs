using System.Net.Http.Headers;

using Diabits.Web.Infrastructure.Services.Auth;

namespace Diabits.Web.Infrastructure.Api;

/// <summary>
/// HTTP message handler that automatically attaches JWT bearer token to requests.
/// </summary>
public class AuthorizationHandler(TokenStorage tokens) : DelegatingHandler
{
    private readonly TokenStorage _tokens = tokens;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var session = await _tokens.LoadAsync();
        if (session is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        }

        return await base.SendAsync(request, ct);
    }
}
