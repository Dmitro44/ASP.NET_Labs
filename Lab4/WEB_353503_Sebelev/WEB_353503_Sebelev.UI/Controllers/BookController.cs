using Microsoft.AspNetCore.Mvc;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;
using WEB_353503_Sebelev.UI.Services.BookCategoryService;
using WEB_353503_Sebelev.UI.Services.BookService;
using WEB_353503_Sebelev.UI.ViewModels;

namespace WEB_353503_Sebelev.UI.Controllers;

public class BookController : Controller
{
    private readonly IBookCategoryService _bookCategoryService;
    private readonly IBookService _bookService;

    public BookController(IBookCategoryService bookCategoryService, IBookService bookService)
    {
        _bookCategoryService = bookCategoryService;
        _bookService = bookService;
    }

    public async Task<IActionResult> Index(string? category, int pageNo = 1)
    {
        var productResponse = await _bookService.GetBookListAsync(category, pageNo);
        var categoryList = await _bookCategoryService.GetCategoryListAsync();
        if (!productResponse.Successfull)
        {
            return NotFound(productResponse.ErrorMessage);
        }

        ViewData["currentNormalizedCategory"] = category;

        if (category != null)
        {
            var currentCategory = categoryList.Data.Find(c => c.NormalizedName == category);
            ViewData["currentCategory"] = currentCategory.Name;
        }

        var bookListViewModel = new BookListViewModel
        {
            Books = productResponse.Data, 
            Categories = categoryList.Data
        };
        
        return View(bookListViewModel);
    }
}