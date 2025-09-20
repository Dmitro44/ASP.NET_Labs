using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.UI.Services.BookCategoryService;
using WEB_353503_Sebelev.UI.Services.BookService;

namespace WEB_353503_Sebelev.UI.Areas.Admin.Pages
{
    public class UpdatePageModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly IBookCategoryService _bookCategoryService;

        public UpdatePageModel(IBookService bookService, IBookCategoryService bookCategoryService)
        {
            _bookService = bookService;
            _bookCategoryService = bookCategoryService;
        }

        [BindProperty]
        public Book Book { get; set; } = null!;
        [BindProperty]
        public IFormFile? Image { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _bookService.GetBookByIdAsync(id.Value);
            
            if (!response.Successfull || response.Data is null)
            {
                return NotFound();
            }
            
            Book = response.Data; 
            
            var categoryResponse = await _bookCategoryService.GetCategoryListAsync();
            if (!categoryResponse.Successfull)
            {
                return NotFound(categoryResponse.ErrorMessage);
            }
            
            ViewData["CategoryId"] = new SelectList(categoryResponse.Data, "Id", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            /*
            _context.Attach(Book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(Book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }*/

            await _bookService.UpdateBookAsync(Book.Id, Book, Image);

            return RedirectToPage("./Index");
        }

        /*private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }*/
    }
}
