using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using WEB_353503_Sebelev.UI.HelperClasses;
using WEB_353503_Sebelev.UI.Services.FileService;

namespace WEB_353503_Sebelev.UI.Services.Authentication;

public class KeycloakAuthService(
    HttpClient httpClient,
    IOptions<KeycloakData> options,
    ITokenAccessor accessor,
    IFileService fileService)
{

    public async Task<(bool Result, string Message)> RegisterUserAsync(
        string email,
        string password,
        IFormFile? avatar)
    {
        try
        {
            await accessor.SetAuthorizationHeaderAsync(httpClient, true);
        }
        catch (Exception e)
        {
           return (false, e.Message); 
        }
        
        var avatarUrl = "/images/default-profile-image.png";

        if (avatar != null)
        {
            avatarUrl = await fileService.SaveFileAsync(avatar);
        }
        
        var newUser = new CreateUserModel
        {
            Email = email,
            Username = email
        };
        
        newUser.Attributes.Add("avatar", avatarUrl);
        newUser.Credentials.Add(new UserCredentials { Value = password });

        var requestUri = $"{options.Value.Host}/admin/realms/{options.Value.Realm}/users";

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var userData = JsonSerializer.Serialize(newUser, serializerOptions);
        HttpContent content = new StringContent(userData, Encoding.UTF8, "application/json");
        
        var response = await httpClient.PostAsync(requestUri, content);

        if (response.IsSuccessStatusCode)
        {
            return (true, string.Empty);
        }
        
        return (false, response.StatusCode.ToString());
    } 
}

internal class CreateUserModel
{
    public Dictionary<string, string> Attributes { get; set; } = new();
    public string Username { get; set; }
    public string Email { get; set; }
    public bool Enabled { get; set; }
    public bool EmailVerified { get; set; }
    public List<UserCredentials> Credentials { get; set; } = new();
}

internal class UserCredentials
{
    public string Type { get; set; } = "password";
    public bool Temporary { get; set; } = false;
    public string Value { get; set; }
}