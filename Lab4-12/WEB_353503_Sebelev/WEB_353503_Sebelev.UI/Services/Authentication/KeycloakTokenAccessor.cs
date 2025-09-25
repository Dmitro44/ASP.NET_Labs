using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using WEB_353503_Sebelev.UI.HelperClasses;

namespace WEB_353503_Sebelev.UI.Services.Authentication;

public class KeycloakTokenAccessor(
    IHttpContextAccessor httpContextAccessor,
    IOptions<KeycloakData> options,
    HttpClient httpClient) : ITokenAccessor
{

    public async Task SetAuthorizationHeaderAsync(HttpClient client, bool isClient)
    {
        string token = isClient
            ? await GetClientToken()
            : await GetUserToken();

        client
            .DefaultRequestHeaders
            .Authorization = new AuthenticationHeaderValue("bearer", token);
    }

    private async Task<string> GetUserToken()
    {
        var context = httpContextAccessor.HttpContext;
        var authSession = await context.AuthenticateAsync("keycloak");
        if (authSession.Principal == null)
        {
            throw new AuthenticationFailureException("Пользователь неавторизован");
        }
        return await context.GetTokenAsync("keycloak", "access_token");
    }

    private async Task<string> GetClientToken()
    {
        var requestUri = $"{options.Value.Host}/realms/{options.Value.Realm}/protocol/openid-connect/token";

        HttpContent content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("client_id", options.Value.ClientId),
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_secret", options.Value.ClientSecret)
        ]);
        
        var response = await httpClient.PostAsync(requestUri, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(response.StatusCode.ToString());
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        return JsonObject.Parse(jsonString)["access_token"].GetValue<string>();
    }
}