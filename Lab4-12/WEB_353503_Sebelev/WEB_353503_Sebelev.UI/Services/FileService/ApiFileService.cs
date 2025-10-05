using WEB_353503_Sebelev.UI.Services.Authentication;

namespace WEB_353503_Sebelev.UI.Services.FileService;

public class ApiFileService(
    HttpClient httpClient, 
    ITokenAccessor tokenAccessor,
    ILogger<ApiFileService> logger) : IFileService
{
    public async Task<string> SaveFileAsync(IFormFile? file)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{httpClient.BaseAddress}")
        };

        var content = new MultipartFormDataContent();

        if (file is not null)
        {
            var streamContent = new StreamContent(file.OpenReadStream());
            content.Add(streamContent, "file", file.FileName);
        }

        request.Content = content;

        try
        {
            await tokenAccessor.SetAuthorizationHeaderAsync(httpClient, true);
        }
        catch (Exception e)
        {
            logger.LogError("Authorization header was not added. Error: {EMessage}", e.Message);
            return string.Empty;
        }

        var response = await httpClient.SendAsync(
            request,
            CancellationToken.None);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("-----> file was uploaded.");
            return await response.Content.ReadFromJsonAsync<string>();
        }
        
        logger.LogError($"-----> file was not uploaded. Error: " +
                         $"{response.StatusCode.ToString()}");     
        return string.Empty;
    }

    public async Task DeleteFileAsync(string fileName)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{httpClient.BaseAddress}?imagePath=" + fileName)
        };
        
        var response = await httpClient.SendAsync(
            request,
            CancellationToken.None);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("-----> file was deleted.");
        }
        else
        {
            logger.LogError($"-----> file was not deleted. Error: " +
                            $"{response.StatusCode.ToString()}");
        }
    }
}