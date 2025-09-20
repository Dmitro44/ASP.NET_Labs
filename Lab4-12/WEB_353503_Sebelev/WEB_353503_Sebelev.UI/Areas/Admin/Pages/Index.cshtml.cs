using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;
using WEB_353503_Sebelev.UI;
using WEB_353503_Sebelev.UI.Services.BookService;

namespace WEB_353503_Sebelev.UI.Areas.Admin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IBookService _bookService;
        public string? ErrorMessage { get; set; }

        public IndexModel(IBookService bookService)
        {
            _bookService = bookService;
        }

        public ListModel<Book> Books { get;set; } = null!;

        public async Task OnGetAsync(int pageNo)
        {
            var response = await _bookService.GetBookListAsync(null, pageNo);

            if (!response.Successfull)
            {
                ErrorMessage = response.ErrorMessage;
                return;
            }

            Books = response.Data;
        }
    }
}
