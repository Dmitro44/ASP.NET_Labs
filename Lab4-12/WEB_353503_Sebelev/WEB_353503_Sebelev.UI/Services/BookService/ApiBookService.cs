using System.Text;
using System.Text.Json;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;

namespace WEB_353503_Sebelev.UI.Services.BookService;

public class ApiBookService : IBookService
{
    private readonly HttpClient _httpClient;
    private readonly string _pageSize;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ILogger<ApiBookService> _logger;

    public ApiBookService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ApiBookService> logger)
    {
        _httpClient = httpClient;
        _pageSize = configuration.GetSection("ItemsPerPage").Value;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        _logger = logger;
    }
    
    public async Task<ResponseData<ListModel<Book>>> GetBookListAsync(string? categoryNormalizedSize, int pageNo = 1)
    {
        var urlString = new StringBuilder($"{_httpClient.BaseAddress.AbsoluteUri}");
        
        if (!urlString.ToString().EndsWith('/')) urlString.Append('/');

        if (categoryNormalizedSize != null)
        {
            urlString.Append($"{categoryNormalizedSize}/");
        }

        var queryParams = new List<string>();

        if (pageNo > 1)
        {
            queryParams.Add($"pageNo={pageNo}");
        }
        
        if (!_pageSize.Equals("3"))
        {
            queryParams.Add($"pageSize={_pageSize}");
        }

        if (queryParams.Count > 0)
        {
            urlString.Append($"?{string.Join("&", queryParams)}");
        }
        
        var response = await _httpClient.GetAsync(new Uri(urlString.ToString()));

        if (response.IsSuccessStatusCode)
        {
            try
            {
                return await response
                    .Content
                    .ReadFromJsonAsync<ResponseData<ListModel<Book>>>(_serializerOptions);
            }
            catch(JsonException ex)
            {
               _logger.LogError($"-----> Ошибка: {ex.Message}"); 
               
               return ResponseData<ListModel<Book>>
                   .Error($"Ошибка: {ex.Message}");
            }
        }
        
        _logger.LogError($"-----> Данные не получены от сервера. Error: " +
                         $"{response.StatusCode.ToString()}");
        return ResponseData<ListModel<Book>>
            .Error($"Данные не получены от сервера. Error: " +
                   $"{response.StatusCode.ToString()}");
    }

    public async Task<ResponseData<Book?>> GetBookByIdAsync(int id)
    {
        var urlString = new StringBuilder($"{_httpClient.BaseAddress.AbsoluteUri}");
        
        if (!urlString.ToString().EndsWith('/')) urlString.Append('/');
        
        urlString.Append($"{id}");
        
        var response = await _httpClient.GetAsync(new Uri(urlString.ToString()));

        if (response.IsSuccessStatusCode)
        {
            try
            {
                return await response
                    .Content
                    .ReadFromJsonAsync<ResponseData<Book>>(_serializerOptions);
            }
            catch(JsonException ex)
            {
                _logger.LogError($"-----> Ошибка: {ex.Message}"); 
               
                return ResponseData<Book?>
                    .Error($"Ошибка: {ex.Message}");
            }
        }
        
        _logger.LogError($"-----> Данные не получены от сервера. Error: " +
                         $"{response.StatusCode.ToString()}");
        return ResponseData<Book?>
            .Error($"Данные не получены от сервера. Error: " +
                   $"{response.StatusCode.ToString()}");
    }

    public async Task UpdateBookAsync(int id, Book book, IFormFile? formFile)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"{_httpClient.BaseAddress}/{id}"),
        };
        
        var content = new MultipartFormDataContent();
        
        if (formFile is not null)
        {
            var streamContent = new StreamContent(formFile.OpenReadStream());
            content.Add(streamContent, "file", formFile.FileName);
        }
        
        var data = new StringContent(JsonSerializer.Serialize(book));
        content.Add(data, "book");

        request.Content = content;
        var response = await _httpClient.SendAsync(
            request,
            CancellationToken.None);
        
        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("-----> object was updated.");
        }
        else
        {
            _logger.LogError($"-----> object not updated. Error: " +
                             $"{response.StatusCode.ToString()}");     
        }
    }

    public async Task DeleteBookAsync(int id)
    {
        var requst = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"{_httpClient.BaseAddress}/{id}"),
        };
        
        var response = await _httpClient.SendAsync(requst, CancellationToken.None);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("-----> object was deleted.");
        }
        else
        {
            _logger.LogError($"-----> object not deleted. Error: " +
                             $"{response.StatusCode.ToString()}");
        } 
    }

    public async Task<ResponseData<Book>> CreateBookAsync(Book book, IFormFile? formFile)
    {
        // book.Image = "Image/noimage.jpg";

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{_httpClient.BaseAddress}/")
        };
        var content = new MultipartFormDataContent();

        if (formFile is not null)
        {
            var streamContent = new StreamContent(formFile.OpenReadStream());
            content.Add(streamContent, "file", formFile.FileName);
        }
        
        var data = new StringContent(JsonSerializer.Serialize(book));
        content.Add(data, "book");

        request.Content = content;
        var response = await _httpClient.SendAsync(
            request,
            CancellationToken.None);

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response
                .Content
                .ReadFromJsonAsync<ResponseData<Book>>(_serializerOptions);
            return responseData;
        }
        
        _logger.LogError($"-----> object not created. Error: " +
                         $"{response.StatusCode.ToString()}");
        return ResponseData<Book>.Error($"Объект не добавлен. Error: " +
                                        $"{response.StatusCode.ToString()}");
    }
}