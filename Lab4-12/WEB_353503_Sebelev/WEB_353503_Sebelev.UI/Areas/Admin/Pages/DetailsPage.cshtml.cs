using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.UI.Services.BookService;

namespace WEB_353503_Sebelev.UI.Areas.Admin.Pages
{
    public class DetailsPageModel : PageModel
    {
        private readonly IBookService _bookService;

        public DetailsPageModel(IBookService bookService)
        {
            _bookService = bookService;
        }

        public Book Book { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _bookService.GetBookByIdAsync(id.Value);

            if (!response.Successfull || response.Data is null) 
                return NotFound();
            
            Book = response.Data;

            return Page();
        }
    }
}
