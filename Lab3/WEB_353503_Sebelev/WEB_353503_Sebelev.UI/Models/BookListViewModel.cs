using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;

namespace WEB_353503_Sebelev.UI.ViewModels;

public class BookListViewModel
{
    public ListModel<Book> Books { get; set; }
    public IEnumerable<Category> Categories { get; set; }
}