using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.UI.Services.BookService;

namespace WEB_353503_Sebelev.UI.Areas.Admin.Pages
{
    public class DeletePageModel : PageModel
    {
        private readonly IBookService _bookService;
        
        public DeletePageModel(IBookService bookService)
        {
            _bookService = bookService;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            /*var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                Book = book;
                _context.Books.Remove(Book);
                await _context.SaveChangesAsync();
            }*/
            
            await _bookService.DeleteBookAsync(id.Value);

            return RedirectToPage("./Index");
        }
    }
}
