using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace WEB_Lab13.UI.Services.Auth;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> Register(string username, string displayName, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register",
            new { Username = username, DisplayName = displayName, Password = password });

        if (response.IsSuccessStatusCode)
        {
            var obj = await response.Content.ReadFromJsonAsync<TokenResponse>();
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", obj.Token);
            (_authStateProvider as JwtAuthStateProvider)?.NotifyUserAuthentication(obj.Token);
            return true;
        }
        
        return false;
    }

    public async Task<bool> Login(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login",
                new { Username = username, Password = password });

        if (response.IsSuccessStatusCode)
        {
            var obj = await response.Content.ReadFromJsonAsync<TokenResponse>();
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", obj.Token);
            (_authStateProvider as JwtAuthStateProvider)?.NotifyUserAuthentication(obj.Token);
            return true;
        }

        return false;
    }

    public async Task Logout()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        (_authStateProvider as JwtAuthStateProvider)?.NotifyUserLogout();
    }
}

public class TokenResponse
{
    public string Token { get; set; }
}