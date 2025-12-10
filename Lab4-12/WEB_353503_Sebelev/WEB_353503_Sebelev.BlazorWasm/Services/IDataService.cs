using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;

namespace WEB_353503_Sebelev.BlazorWasm.Services;

public interface IDataService
{
    event Action DataLoaded;
    
    List<Category> Categories { get; set; }
    ListModel<Book> Books { get; set; }
    
    bool Success { get; set; }
    string ErrorMessage { get; set; }
    int TotalPages { get; set; }
    int CurrentPage { get; set; }
    Category SelectedCategory { get; set; }

    public Task GetBookListAsync(int pageNo = 1);

    public Task GetCategoryListAsync();
}