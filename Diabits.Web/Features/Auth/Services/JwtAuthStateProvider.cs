using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Diabits.Web.Features.Auth.Services;

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

        var claims = ParseClaims(token);
        var identity = new ClaimsIdentity(claims, authenticationType: "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyAuthenticationStateChanged() =>
    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    private static IEnumerable<Claim> ParseClaims(JwtSecurityToken token)
    {
        foreach (var claim in token.Claims)
        {
            if(claim.Type == "sub")
            {
                yield return new Claim(ClaimTypes.NameIdentifier, claim.Value);
                yield return claim;
            }
            else if (claim.Type == "role")
            {
                yield return new Claim(ClaimTypes.Role, claim.Value);
            }
            else
            {
                yield return claim;
            }            
        }
    }
}
