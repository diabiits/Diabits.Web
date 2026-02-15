using Microsoft.AspNetCore.Components.Authorization;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Diabits.Web.Services.Auth;

//TODO Implement dialog that asks user to log back in when access token expires
public class JwtAuthStateProvider(TokenStorage tokens) : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var session = await tokens.LoadAsync();
        if (session is null)
            return new AuthenticationState(Anonymous);

        var token = TokenHandler.ReadJwtToken(session.AccessToken);
        if (token.ValidTo < DateTime.UtcNow)
        {
            await tokens.ClearAsync();
            return new AuthenticationState(Anonymous);
        }

        var identity = new ClaimsIdentity(token.Claims, authenticationType: "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyAuthStateChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}