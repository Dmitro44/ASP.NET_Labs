using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;
using WEB_353503_Sebelev.UI.Services.BookCategoryService;

namespace WEB_353503_Sebelev.UI.Services.BookService;

public class MemoryBookService : IBookService
{
    private List<Book> _books;
    private List<Category>? _categories;

    public MemoryBookService(IBookCategoryService categoryService)
    {
        _categories = categoryService.GetCategoryListAsync()
            .Result.Data;

        SetupData();
    }

    private void SetupData()
    {
        _books = new List<Book>
        {
            new Book
            {
                Id = 1, Title = ""
            }
        };
    }
    
    public Task<ResponseData<ListModel<Book>>> GetBookListAsync(string? categoryNormalizedSize, int pagoNo = 1)
    {
        throw new NotImplementedException();
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

    public Task<ResponseData<Book>> CreateBookAsync(Book book, IFormFile? formFile)
    {
        throw new NotImplementedException();
    }
}