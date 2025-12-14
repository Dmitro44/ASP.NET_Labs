using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using WEB_Lab13.UI;
using WEB_Lab13.UI.Services.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => {
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("API");
    var jsRuntime = sp.GetRequiredService<IJSRuntime>();
    var authStateProvider = sp.GetRequiredService<AuthenticationStateProvider>();
    return new AuthService(httpClient, jsRuntime, authStateProvider);
});
builder.Services.AddScoped<AuthMessageHandler>();

builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiUrl"] ?? "");
});
builder.Services.AddScoped(sp => {
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("API");
    var jsRuntime = sp.GetRequiredService<IJSRuntime>();
    return new GameClient(httpClient, jsRuntime);
});

await builder.Build().RunAsync();
