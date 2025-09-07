using Microsoft.EntityFrameworkCore;
using WEB_353503_Sebelev.Domain.Entities;

namespace WEB_353503_Sebelev.API.Data;

public static class DbInitializer
{
    public static async Task SeedData(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var baseUrl = app.Configuration["AppUrl"];

        await context.Database.MigrateAsync();

        if (await context.Categories.AnyAsync() || await context.Books.AnyAsync())
        {
            return;
        } 

        var categories = new List<Category>
        {
            new Category
            {
                Id = 1,
                Name = "Фантастика",
                NormalizedName = "fantasy"
            },
            new Category
            {
                Id = 2,
                Name = "Детектив",
                NormalizedName = "detective"
            },
            new Category
            {
                Id = 3,
                Name = "Приключение",
                NormalizedName = "adventure"
            },
            new Category
            {
                Id = 4,
                Name = "Романтика",
                NormalizedName = "romance"
            },
            new Category
            {
                Id = 5,
                Name = "Ужасы",
                NormalizedName = "horror"
            },
            new Category
            {
                Id = 6,
                Name = "Мистика",
                NormalizedName = "mysticism"
            }
        };
        
        await context.Categories.AddRangeAsync(categories);

        var books = new List<Book>
        {
            new Book
            {
                Id = 1, Title = "Автостопом по галактике",
                Description = "Комическое путешествие по космосу после гибели Земли",
                Author = "Дуглас Адамс",
                Price = 100, Image = $"{baseUrl}/Images/The-Hitchhiker_s-Guide-to-the-Galaxy.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("fantasy"))
            },
            new Book
            {
                Id = 2, Title = "Приключения Шерлока Холмса",
                Author = "Артур Конан Дойл",
                Description = "Гениальный сыщик решает запутанные головоломки",
                Price = 150, Image = $"{baseUrl}/Images/The-Adventure-of-Sherlock-Holmes.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("detective"))
            },
            new Book
            {
                Id = 3, Title = "Остров Сокровищ",
                Author = "Роберт Льюис Стивенсон",
                Description = "Поиски пиратского клада, полные опасностей",
                Price = 120, Image = $"{baseUrl}/Images/Treasure-Island.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("adventure"))
            },
            new Book
            {
                Id = 4, Title = "Анна Каренина",
                Author = "Лев Толстой",
                Description = "Трагическая любовь замужней дамы в высшем свете",
                Price = 200, Image = $"{baseUrl}/Images/Anna-Carenina.png",
                Category = categories.Find(c => c.NormalizedName.Equals("romance"))
            },
            new Book
            {
                Id = 5, Title = "Дракула",
                Author = "Брэм Стокер",
                Description = "Классическая история о самом известном вампире",
                Price = 300, Image = $"{baseUrl}/Images/Dracula.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("horror"))
            },
            new Book
            {
                Id = 6, Title = "Мастер и Маргарита",
                Author = "Михаил Булгаков",
                Description = "Визит дьявола в Москву 1930-х годов",
                Price = 500, Image = $"{baseUrl}/Images/Master-and-Margarita.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("mysticism"))
            }
        };
        
        await context.Books.AddRangeAsync(books);
        
        await context.SaveChangesAsync();
    }
}