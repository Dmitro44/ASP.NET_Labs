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

    public Task<ResponseData<Book>> GetBookByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateBookAsync(int id, Book book, IFormFile? formFile)
    {
        throw new NotImplementedException();
    }

    public Task DeleteBookAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseData<Book>> CreateBookAsync(Book book, IFormFile? formFile)
    {
        // book.Image = "Image/noimage.jpg";

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = _httpClient.BaseAddress
        };

        request.Content = new StringContent(JsonSerializer.Serialize(book));
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