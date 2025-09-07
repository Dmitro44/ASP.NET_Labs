using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;

namespace WEB_353503_Sebelev.UI.Services.BookCategoryService;

public interface IBookCategoryService
{
    public Task<ResponseData<List<Category>>> GetCategoryListAsync();
}