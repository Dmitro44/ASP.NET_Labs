using System.Text;
using System.Text.Json;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;

namespace WEB_353503_Sebelev.UI.Services.BookCategoryService;

public class ApiBookCategoryService : IBookCategoryService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ILogger<ApiBookCategoryService> _logger;

    public ApiBookCategoryService(
        HttpClient httpClient,
        ILogger<ApiBookCategoryService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
    public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
    {
        var urlString = $"{_httpClient.BaseAddress.AbsoluteUri}/";
        Console.WriteLine(urlString);
        
        var request = await _httpClient.GetAsync(new Uri(urlString));

        if (request.IsSuccessStatusCode)
        {
            try
            {
                return await request
                    .Content
                    .ReadFromJsonAsync<ResponseData<List<Category>>>(_serializerOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogError($"-----> Ошибка: {ex.Message}");
                
                return ResponseData<List<Category>>.Error($"Ошибка: {ex.Message}");
            }
        }
        
        _logger.LogError($"-----> Данные не получены от сервера. Error: " +
                         $"{request.StatusCode.ToString()}");
        
        return ResponseData<List<Category>>.Error($"Данные не получены от сервера. Error: " +
                                                  $"{request.StatusCode.ToString()}");
    }
}