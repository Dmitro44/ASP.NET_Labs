using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WEB_353503_Sebelev.API.Data;
using WEB_353503_Sebelev.API.UseCases;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;

namespace WEB_353503_Sebelev.Tests;

public class MediatrTests : IDisposable
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<AppDbContext> _contextOptions;

    public MediatrTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new AppDbContext(_contextOptions);

        context.Database.EnsureCreated();

        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Фантастика", NormalizedName = "fantasy" },
            new Category { Id = 2, Name = "Детектив", NormalizedName = "detective" },
            new Category { Id = 3, Name = "Приключение", NormalizedName = "adventure" },
            new Category { Id = 4, Name = "Романтика", NormalizedName = "romance" },
            new Category { Id = 5, Name = "Ужасы", NormalizedName = "horror" },
            new Category { Id = 6, Name = "Мистика", NormalizedName = "mysticism" }
        };

        context.Categories.AddRange(categories);

        context.Books.AddRange(
            new Book
            {
                Title = "Автостопом по галактике",
                Description = "Комическое путешествие по космосу после гибели Земли",
                Author = "Дуглас Адамс",
                Price = 100, Image = $"/images/b7c0ee8b-97a4-4928-bfc2-ae7a7ea9c5e5.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("fantasy"))
            },
            new Book
            {
                Title = "Приключения Шерлока Холмса",
                Author = "Артур Конан Дойл",
                Description = "Гениальный сыщик решает запутанные головоломки",
                Price = 150, Image = $"/images/ba069570-fe3a-47d8-b6eb-98bf259707c2.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("detective"))
            },
            new Book
            {
                Title = "Остров Сокровищ",
                Author = "Роберт Льюис Стивенсон",
                Description = "Поиски пиратского клада, полные опасностей",
                Price = 120, Image = $"/images/Treasure-Island.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("adventure"))
            },
            new Book
            {
                Title = "Анна Каренина",
                Author = "Лев Толстой",
                Description = "Трагическая любовь замужней дамы в высшем свете",
                Price = 200, Image = $"/images/Anna-Carenina.png",
                Category = categories.Find(c => c.NormalizedName.Equals("romance"))
            },
            new Book
            {
                Title = "Дракула",
                Author = "Брэм Стокер",
                Description = "Классическая история о самом известном вампире",
                Price = 300, Image = $"/images/Dracula.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("horror"))
            },
            new Book
            {
                Title = "Мастер и Маргарита",
                Author = "Михаил Булгаков",
                Description = "Визит дьявола в Москву 1930-х годов",
                Price = 500, Image = $"/images/Master-and-Margarita.jpeg",
                Category = categories.Find(c => c.NormalizedName.Equals("mysticism"))
            }
        );

        context.SaveChanges();
    }

    AppDbContext CreateDbContext() => new AppDbContext(_contextOptions);

    public void Dispose() => _connection.Dispose();


    [Fact]
    public async Task GetListOfBooks_ShouldReturnFirstPageOfThreeItems()
    {
        // Arrange
        await using var context = CreateDbContext();

        var handler = new GetListOfBooksHandler(context);

        var command = new GetListOfBooks(null);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.IsType<ResponseData<ListModel<Book>>>(result);
        Assert.True(result.Successfull);
        Assert.Equal(1, result.Data.CurrentPage);
        Assert.Equal(3, result.Data.Items.Count);
        Assert.Equal(2, result.Data.TotalPages);
        Assert.Equal(context.Books.First(), result.Data.Items[0]);
    }
    
    [Fact]
    public async Task GetListOfBooks_ShouldCorrectlyPickPageNo()
    {
        // Arrange
        await using var context = CreateDbContext();

        var handler = new GetListOfBooksHandler(context);

        var command = new GetListOfBooks(null, 2);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.IsType<ResponseData<ListModel<Book>>>(result);
        Assert.True(result.Successfull);
        Assert.Equal(2, result.Data.CurrentPage);
    }
    
    [Fact]
    public async Task GetListOfBooks_ShouldFilterBooksCorrectly()
    {
        // Arrange
        await using var context = CreateDbContext();

        var handler = new GetListOfBooksHandler(context);

        var command = new GetListOfBooks("adventure");
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.IsType<ResponseData<ListModel<Book>>>(result);
        Assert.True(result.Successfull);
        Assert.Equal(1, result.Data.CurrentPage);
        Assert.Single(result.Data.Items);
        Assert.Equal(1, result.Data.TotalPages);
        Assert.Equal("adventure", result.Data.Items[0].Category.NormalizedName);
    }
    
    [Fact]
    public async Task GetListOfBooks_ShouldNotAllowSetPageSizeGreaterThanMaxPageSize()
    {
        // Arrange
        await using var context = CreateDbContext();

        var handler = new GetListOfBooksHandler(context);

        var command = new GetListOfBooks(null, PageSize: 25);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.IsType<ResponseData<ListModel<Book>>>(result);
        Assert.False(result.Successfull);
        Assert.Equal("Page size exceeds max page size", result.ErrorMessage);
    }
    
    
    [Fact]
    public async Task GetListOfBooks_ShouldReturnNotSuccessfulIfPageNoGreaterThanPageCount()
    {
        // Arrange
        await using var context = CreateDbContext();

        var handler = new GetListOfBooksHandler(context);

        var command = new GetListOfBooks(null, 5);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.IsType<ResponseData<ListModel<Book>>>(result);
        Assert.False(result.Successfull);
        Assert.Equal("Page out of range", result.ErrorMessage);
    }}