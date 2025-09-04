using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;

namespace WEB_353503_Sebelev.UI.Services.BookCategoryService;

public class MemoryBookCategoryService : IBookCategoryService
{
    public Task<ResponseData<List<Category>>> GetCategoryListAsync()
    {
        var categories = new List<Category>
        {
            new Category
            {
                Id = 1,
                Name = "Фантастика",
                NormalizedName = "fantasy"
            },
            new Category
            {
                Id = 2,
                Name = "Детектив",
                NormalizedName = "detective"
            },
            new Category
            {
                Id = 3,
                Name = "Приключение",
                NormalizedName = "adventure"
            },
            new Category
            {
                Id = 4,
                Name = "Романтика",
                NormalizedName = "romance"
            },
            new Category
            {
                Id = 5,
                Name = "Ужасы",
                NormalizedName = "horror"
            },
            new Category
            {
                Id = 6,
                Name = "Мистика",
                NormalizedName = "mysticism"
            }
        };
        
        var result = ResponseData<List<Category>>.Success(categories);
        
        return Task.FromResult(result);
    }
}