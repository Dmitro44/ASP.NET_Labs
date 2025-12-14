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
        
        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        
        try
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            
            var jwtToken = handler.ReadJwtToken(token); 

            var identity = new ClaimsIdentity(jwtToken.Claims, "JwtAuth");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch (Microsoft.IdentityModel.Tokens.SecurityTokenMalformedException)
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public void NotifyUserAuthentication(string token) =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    
    public void NotifyUserLogout() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}