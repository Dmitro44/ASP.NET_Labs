using Microsoft.AspNetCore.Mvc;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;
using WEB_353503_Sebelev.UI.Extensions;
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

    [HttpGet]
    [Route("Catalog/{category?}")]
    public async Task<IActionResult> Index(string? category, int pageNo = 1)
    {
        var categoryList = await _bookCategoryService.GetCategoryListAsync();
        if (!categoryList.Successfull)
            return NotFound(categoryList.ErrorMessage);
        
        var productResponse = await _bookService.GetBookListAsync(category, pageNo);
        if (!productResponse.Successfull)
            return NotFound(productResponse.ErrorMessage);

        if (category != null)
        {
            var currentCategory = categoryList.Data.Find(c => c.NormalizedName == category);
            ViewData["currentCategory"] = currentCategory.Name;
        }

        ViewData["currentCategory"] = category;

        var bookListViewModel = new BookListViewModel
        {
            Books = productResponse.Data, 
            Categories = categoryList.Data
        };

        if (Request.IsAjaxRequest())
            return PartialView("_BookListPartial", bookListViewModel);
        
        return View(bookListViewModel);
    }
}