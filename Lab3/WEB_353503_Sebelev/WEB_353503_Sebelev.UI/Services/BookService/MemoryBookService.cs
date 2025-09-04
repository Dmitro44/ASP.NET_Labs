using Microsoft.AspNetCore.Mvc;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;
using WEB_353503_Sebelev.UI.Services.BookCategoryService;

namespace WEB_353503_Sebelev.UI.Services.BookService;

public class MemoryBookService : IBookService
{
    private List<Book> _books;
    private List<Category>? _categories;
    private readonly IConfiguration _configuration;

    public MemoryBookService(
        [FromServices] IConfiguration config,
        IBookCategoryService categoryService)
    {
        _categories = categoryService.GetCategoryListAsync()
            .Result.Data;
        
        _configuration = config;

        SetupData();
    }

    private void SetupData()
    {
        _books = new List<Book>
        {
            new Book
            {
                Id = 1, Title = "Автостопом по галактике",
                Description = "Комическое путешествие по космосу после гибели Земли",
                Author = "Дуглас Адамс",
                Price = 100, Image = "../Images/The-Hitchhiker_s-Guide-to-the-Galaxy.jpeg",
                Category = _categories.Find(c => c.NormalizedName.Equals("fantasy"))
            },
            new Book
            {
                Id = 2, Title = "Приключения Шерлока Холмса",
                Author = "Артур Конан Дойл",
                Description = "Гениальный сыщик решает запутанные головоломки",
                Price = 150, Image = "../Images/The-Adventure-of-Sherlock-Holmes.jpeg",
                Category = _categories.Find(c => c.NormalizedName.Equals("detective"))
            },
            new Book
            {
                Id = 3, Title = "Остров Сокровищ",
                Author = "Роберт Льюис Стивенсон",
                Description = "Поиски пиратского клада, полные опасностей",
                Price = 120, Image = "../Images/Treasure-Island.jpeg",
                Category = _categories.Find(c => c.NormalizedName.Equals("adventure"))
            },
            new Book
            {
                Id = 4, Title = "Анна Каренина",
                Author = "Лев Толстой",
                Description = "Трагическая любовь замужней дамы в высшем свете",
                Price = 200, Image = "../Images/Anna-Carenina.png",
                Category = _categories.Find(c => c.NormalizedName.Equals("romance"))
            },
            new Book
            {
                Id = 5, Title = "Дракула",
                Author = "Брэм Стокер",
                Description = "Классическая история о самом известном вампире",
                Price = 300, Image = "../Images/Dracula.jpeg",
                Category = _categories.Find(c => c.NormalizedName.Equals("horror"))
            },
            new Book
            {
                Id = 6, Title = "Мастер и Маргарита",
                Author = "Михаил Булгаков",
                Description = "Визит дьявола в Москву 1930-х годов",
                Price = 500, Image = "../Images/Master-and-Margarita.jpeg",
                Category = _categories.Find(c => c.NormalizedName.Equals("mysticism"))
            }
        };
    }
    
    public Task<ResponseData<ListModel<Book>>> GetBookListAsync(string? categoryNormalizedName, int pageNo = 1)
    {
        var itemsPerPage = int.Parse(_configuration["ItemsPerPage"]);

        var filteredBooks = _books
            .Where(b => categoryNormalizedName == null ||
                        b.Category.NormalizedName.Equals(categoryNormalizedName))
            .ToList();

        return Task.FromResult(ResponseData<ListModel<Book>>.Success(
            new ListModel<Book>
            {
                Items = filteredBooks
                    .Skip((pageNo - 1) * itemsPerPage)
                    .Take(itemsPerPage)
                    .ToList(),
                CurrentPage = pageNo > 0 && pageNo < filteredBooks.Count ? pageNo : 1,
                TotalPages = (int)Math.Ceiling((double)filteredBooks.Count / itemsPerPage) 
            })
        );
    }

    public Task<ResponseData<Book>> GetBookByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateBookAsync(int id, Book book, IFormFile? formFile)
    {
        throw new NotImplementedException();
    }

    public Task DeleteBookAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseData<Book>> CreateBookAsync(Book book, IFormFile? formFile)
    {
        throw new NotImplementedException();
    }
}