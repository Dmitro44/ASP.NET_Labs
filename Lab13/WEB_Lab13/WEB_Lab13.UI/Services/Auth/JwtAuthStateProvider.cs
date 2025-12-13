using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace WEB_Lab13.UI.Services.Auth;

public class JwtAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _js;

    public JwtAuthStateProvider(IJSRuntime js)
    {
        _js = js;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
        ClaimsIdentity identity = new();

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            identity = new ClaimsIdentity(jwt.Claims, "jwt");
        }
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyUserAuthentication(string token) =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    
    public void NotifyUserLogout() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}