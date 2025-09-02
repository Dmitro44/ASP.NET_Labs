using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;

namespace WEB_353503_Sebelev.UI.Services.BookService;

public interface IBookService
{
    public Task<ResponseData<ListModel<Book>>> GetBookListAsync(string? categoryNormalizedSize, int pagoNo = 1);
    public Task<ResponseData<Book>> GetBookByIdAsync(int id);
    public Task UpdateBookAsync(int id, Book book, IFormFile? formFile);
    public Task DeleteBookAsync(int id);
    
    public Task<ResponseData<Book>> CreateBookAsync(Book book, IFormFile? formFile);
}