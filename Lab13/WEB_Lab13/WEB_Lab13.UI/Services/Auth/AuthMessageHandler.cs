using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace WEB_Lab13.UI.Services.Auth;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;

    public AuthMessageHandler(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", cancellationToken, "authToken");
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        
        return await base.SendAsync(request, cancellationToken);
    }
}