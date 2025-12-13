using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Http;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;

namespace WEB_353503_Sebelev.BlazorWasm.Services;

public class DataService : IDataService
{
    private readonly HttpClient _httpClient;
    private readonly string _pageSize;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ILogger<DataService> _logger;
    private readonly IAccessTokenProvider _tokenProvider;

    public DataService(ILogger<DataService> logger, IConfiguration configuration, HttpClient httpClient, IAccessTokenProvider tokenProvider)
    {
        _logger = logger;
        _httpClient = httpClient;
        _tokenProvider = tokenProvider;
        _pageSize = configuration.GetSection("ItemsPerPage").Value;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public event Action? DataLoaded;
    public List<Category> Categories { get; set; } = new();
    public ListModel<Book> Books { get; set; } = new();
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public Category? SelectedCategory { get; set; } = null;
    
    public async Task GetBookListAsync(int pageNo = 1)
    {
        var urlString = new StringBuilder($"{_httpClient.BaseAddress.AbsoluteUri + "Book"}");
        
        if (!urlString.ToString().EndsWith('/')) urlString.Append('/');

        if (SelectedCategory?.NormalizedName is not null)
        {
            urlString.Append($"{SelectedCategory.NormalizedName}/");
        }

        List<KeyValuePair<string, string>> queryParams = new ();

        if (pageNo > 1)
        {
            queryParams.Add(KeyValuePair.Create("pageNo", pageNo.ToString()));
        }
        
        if (!_pageSize.Equals("3"))
        {
            queryParams.Add(KeyValuePair.Create("pageSize", _pageSize));
        }

        if (queryParams.Count > 0)
        {
            urlString.Append(QueryString.Create(queryParams));
        }

        var tokenRequest = await _tokenProvider.RequestAccessToken();
        if (tokenRequest.TryGetToken(out var token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
        }
        
        var response = await _httpClient.GetAsync(new Uri(urlString.ToString()));

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var result = await response
                    .Content
                    .ReadFromJsonAsync<ResponseData<ListModel<Book>>>(_serializerOptions);

                if (result.Successfull)
                {
                    Books = result.Data;
                    CurrentPage = result.Data.CurrentPage;
                    TotalPages = result.Data.TotalPages;
                    DataLoaded?.Invoke();
                    return;
                }
            }
            catch(JsonException ex)
            {
                _logger.LogError($"-----> Ошибка: {ex.Message}"); 
               
                Success = false;
                ErrorMessage = $"Error: {ex.Message}";
                return;
            }
        }
        
        _logger.LogError($"-----> Данные не получены от сервера. Error: " +
                         $"{response.StatusCode.ToString()}");
        
        Success = false;
        ErrorMessage = $"Error: {response.StatusCode.ToString()}";
    }

    public async Task GetCategoryListAsync()
    {
        var urlString = $"{_httpClient.BaseAddress.AbsoluteUri + "Categories"}/";
        Console.WriteLine(urlString);
    
        var request = await _httpClient.GetAsync(new Uri(urlString));

        if (request.IsSuccessStatusCode)
        {
            try
            {
                var result = await request
                    .Content
                    .ReadFromJsonAsync<ResponseData<List<Category>>>(_serializerOptions);

                if (result.Successfull)
                {
                    Categories = result.Data;
                    DataLoaded?.Invoke();
                    return;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError($"-----> Ошибка: {ex.Message}");
            
                Success = false;
                ErrorMessage = $"Error: {ex.Message}";
                return;
            }
        }
    
        _logger.LogError($"-----> Данные не получены от сервера. Error: " +
                         $"{request.StatusCode.ToString()}");
    
        Success = false;
        ErrorMessage = $"Error: {request.StatusCode.ToString()}";
    }
}