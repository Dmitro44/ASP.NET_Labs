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
                Name = "Фантастика",
                NormalizedName = "fantasy"
            },
            new Category
            {
                Name = "Детектив",
                NormalizedName = "detective"
            },
            new Category
            {
                Name = "Приключение",
                NormalizedName = "adventure"
            },
            new Category
            {
                Name = "Романтика",
                NormalizedName = "romance"
            },
            new Category
            {
                Name = "Ужасы",
                NormalizedName = "horror"
            },
            new Category
            {
                Name = "Мистика",
                NormalizedName = "mysticism"
            }
        };
        
        await context.Categories.AddRangeAsync(categories);

        var books = new List<Book>
        {
            new Book
            {
                Title = "Автостопом по галактике",
                Description = "Комическое путешествие по космосу после гибели Земли",
                Author = "Дуглас Адамс",
                Price = 100, Image = $"{baseUrl}/images/b7c0ee8b-97a4-4928-bfc2-ae7a7ea9c5e5.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("fantasy"))
            },
            new Book
            {
                Title = "Приключения Шерлока Холмса",
                Author = "Артур Конан Дойл",
                Description = "Гениальный сыщик решает запутанные головоломки",
                Price = 150, Image = $"{baseUrl}/images/ba069570-fe3a-47d8-b6eb-98bf259707c2.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("detective"))
            },
            new Book
            {
                Title = "Остров Сокровищ",
                Author = "Роберт Льюис Стивенсон",
                Description = "Поиски пиратского клада, полные опасностей",
                Price = 120, Image = $"{baseUrl}/images/Treasure-Island.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("adventure"))
            },
            new Book
            {
                Title = "Анна Каренина",
                Author = "Лев Толстой",
                Description = "Трагическая любовь замужней дамы в высшем свете",
                Price = 200, Image = $"{baseUrl}/images/Anna-Carenina.png",
                Category = categories.Find(c => c.NormalizedName.Equals("romance"))
            },
            new Book
            {
                Title = "Дракула",
                Author = "Брэм Стокер",
                Description = "Классическая история о самом известном вампире",
                Price = 300, Image = $"{baseUrl}/images/Dracula.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("horror"))
            },
            new Book
            {
                Title = "Мастер и Маргарита",
                Author = "Михаил Булгаков",
                Description = "Визит дьявола в Москву 1930-х годов",
                Price = 500, Image = $"{baseUrl}/images/Master-and-Margarita.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("mysticism"))
            }
        };
        
        await context.Books.AddRangeAsync(books);
        
        await context.SaveChangesAsync();
    }
}