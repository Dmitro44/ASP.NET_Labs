using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.UI.Services.BookCategoryService;
using WEB_353503_Sebelev.UI.Services.BookService;

namespace WEB_353503_Sebelev.UI.Areas.Admin.Pages
{
    public class CreatePageModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly IBookCategoryService _bookCategoryService;

        public CreatePageModel(IBookService bookService, IBookCategoryService bookCategoryService)
        {
            _bookService = bookService;
            _bookCategoryService = bookCategoryService;
        }

        public async Task<IActionResult> OnGet()
        {
            var response = await _bookCategoryService.GetCategoryListAsync();
            if (!response.Successfull)
            {
                return NotFound(response.ErrorMessage);
            }

            ViewData["CategoryId"] = new SelectList(response.Data, "Id", "Name");
            return Page();
        }

        [BindProperty] public Book Book { get; set; } = null!;
        [BindProperty] public IFormFile? Image { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var response = await _bookService.CreateBookAsync(Book, Image);
            if (!response.Successfull)
            {
                return BadRequest(response.ErrorMessage);
            }

            return RedirectToPage("./Index");
        }
    }
}