using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;
using WEB_353503_Sebelev.UI.Controllers;
using WEB_353503_Sebelev.UI.Services.BookCategoryService;
using WEB_353503_Sebelev.UI.Services.BookService;
using WEB_353503_Sebelev.UI.ViewModels;

namespace WEB_353503_Sebelev.Tests;

public class ControllerTests
{
    [Fact]
    public async Task Index_CategoryListNotReceived_ShouldReturn404()
    {
        // Arrange
        var bookCategoryServ = Substitute.For<IBookCategoryService>();
        var bookServ = Substitute.For<IBookService>();

        string? category = null;
        const int pageNo = 2;
        
        ResponseData<List<Category>> categoryList = new ResponseData<List<Category>>
        {
            Data = null,
            ErrorMessage = "Unlak",
            Successfull = false
        };

        bookCategoryServ.GetCategoryListAsync().Returns(categoryList);

        var controller = new BookController(bookCategoryServ, bookServ);

        // Act
        var result = await controller.Index(category, pageNo);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(categoryList.ErrorMessage, notFoundResult.Value);
    }

    [Fact]
    public async Task Index_BookListNotReceived_ShouldReturn404()
    {
        // Arrange
        var bookCategoryServ = Substitute.For<IBookCategoryService>();
        var bookServ = Substitute.For<IBookService>();

        string? category = null;
        const int pageNo = 2;

        ResponseData<List<Category>> categoryList = new ResponseData<List<Category>>
        {
            Data = new List<Category>
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
            },
            ErrorMessage = "Unluck bookCategoryList",
            Successfull = true
        };

        ResponseData<ListModel<Book>> bookList = new ResponseData<ListModel<Book>>
        {
            Data = null,
            ErrorMessage = "Unluck bookList",
            Successfull = false
        };

        bookCategoryServ.GetCategoryListAsync().Returns(categoryList);
        bookServ.GetBookListAsync(category, pageNo).Returns(bookList);

        var controller = new BookController(bookCategoryServ, bookServ);

        // Act
        var result = await controller.Index(category, pageNo);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(bookList.ErrorMessage, notFoundResult.Value);
    }

    [Fact]
    public async Task Index_CategoryListReceived_ListPassedToModel()
    {
        // Arrange
        var bookCategoryServ = Substitute.For<IBookCategoryService>();
        var bookServ = Substitute.For<IBookService>();

        string? category = null;
        const int pageNo = 2;

        var expectedCategories = new List<Category>
        {
            new Category { Name = "Фантастика", NormalizedName = "fantasy" },
            new Category { Name = "Детектив", NormalizedName = "detective" },
            new Category { Name = "Приключение", NormalizedName = "adventure" },
            new Category { Name = "Романтика", NormalizedName = "romance" },
            new Category { Name = "Ужасы", NormalizedName = "horror" },
            new Category { Name = "Мистика", NormalizedName = "mysticism" }
        };

        ResponseData<List<Category>> categoryList = new ResponseData<List<Category>>
        {
            Data = expectedCategories,
            ErrorMessage = "Unluck bookCategoryList",
            Successfull = true
        };

        var expectedBooks = new ListModel<Book>
        {
            CurrentPage = pageNo,
            Items = new List<Book>
            {
                new Book
                {
                    Title = "Автостопом по галактике",
                    Description = "Комическое путешествие по космосу после гибели Земли",
                    Author = "Дуглас Адамс",
                    Price = 100, Image = $"/images/b7c0ee8b-97a4-4928-bfc2-ae7a7ea9c5e5.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("fantasy"))
                },
                new Book
                {
                    Title = "Приключения Шерлока Холмса",
                    Author = "Артур Конан Дойл",
                    Description = "Гениальный сыщик решает запутанные головоломки",
                    Price = 150, Image = $"/images/ba069570-fe3a-47d8-b6eb-98bf259707c2.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("detective"))
                },
                new Book
                {
                    Title = "Остров Сокровищ",
                    Author = "Роберт Льюис Стивенсон",
                    Description = "Поиски пиратского клада, полные опасностей",
                    Price = 120, Image = $"/images/Treasure-Island.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("adventure"))
                },
                new Book
                {
                    Title = "Анна Каренина",
                    Author = "Лев Толстой",
                    Description = "Трагическая любовь замужней дамы в высшем свете",
                    Price = 200, Image = $"/images/Anna-Carenina.png",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("romance"))
                },
                new Book
                {
                    Title = "Дракула",
                    Author = "Брэм Стокер",
                    Description = "Классическая история о самом известном вампире",
                    Price = 300, Image = $"/images/Dracula.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("horror"))
                },
                new Book
                {
                    Title = "Мастер и Маргарита",
                    Author = "Михаил Булгаков",
                    Description = "Визит дьявола в Москву 1930-х годов",
                    Price = 500, Image = $"/images/Master-and-Margarita.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("mysticism"))
                }
            },
            TotalPages = 10
        };

        ResponseData<ListModel<Book>> bookList = new ResponseData<ListModel<Book>>
        {
            Data = expectedBooks,
            ErrorMessage = "Unluck bookList",
            Successfull = true
        };

        bookCategoryServ.GetCategoryListAsync().Returns(categoryList);
        bookServ.GetBookListAsync(category, pageNo).Returns(bookList);
        var controller = new BookController(bookCategoryServ, bookServ);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        // Act
        var result = await controller.Index(category, pageNo);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewModel = Assert.IsType<BookListViewModel>(viewResult.Model);
        Assert.Same(expectedCategories, viewModel.Categories);
    }

    [Fact]
    public async Task Index_BookListReceived_ListPassedToModel()
    {
        // Arrange
        var bookCategoryServ = Substitute.For<IBookCategoryService>();
        var bookServ = Substitute.For<IBookService>();

        string? category = null;
        const int pageNo = 2;

        var expectedCategories = new List<Category>
        {
            new Category { Name = "Фантастика", NormalizedName = "fantasy" },
            new Category { Name = "Детектив", NormalizedName = "detective" },
            new Category { Name = "Приключение", NormalizedName = "adventure" },
            new Category { Name = "Романтика", NormalizedName = "romance" },
            new Category { Name = "Ужасы", NormalizedName = "horror" },
            new Category { Name = "Мистика", NormalizedName = "mysticism" }
        };

        ResponseData<List<Category>> categoryList = new ResponseData<List<Category>>
        {
            Data = expectedCategories,
            ErrorMessage = "Unluck bookCategoryList",
            Successfull = true
        };

        var expectedBooks = new ListModel<Book>
        {
            CurrentPage = pageNo,
            Items = new List<Book>
            {
                new Book
                {
                    Title = "Автостопом по галактике",
                    Description = "Комическое путешествие по космосу после гибели Земли",
                    Author = "Дуглас Адамс",
                    Price = 100, Image = $"/images/b7c0ee8b-97a4-4928-bfc2-ae7a7ea9c5e5.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("fantasy"))
                },
                new Book
                {
                    Title = "Приключения Шерлока Холмса",
                    Author = "Артур Конан Дойл",
                    Description = "Гениальный сыщик решает запутанные головоломки",
                    Price = 150, Image = $"/images/ba069570-fe3a-47d8-b6eb-98bf259707c2.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("detective"))
                },
                new Book
                {
                    Title = "Остров Сокровищ",
                    Author = "Роберт Льюис Стивенсон",
                    Description = "Поиски пиратского клада, полные опасностей",
                    Price = 120, Image = $"/images/Treasure-Island.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("adventure"))
                },
                new Book
                {
                    Title = "Анна Каренина",
                    Author = "Лев Толстой",
                    Description = "Трагическая любовь замужней дамы в высшем свете",
                    Price = 200, Image = $"/images/Anna-Carenina.png",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("romance"))
                },
                new Book
                {
                    Title = "Дракула",
                    Author = "Брэм Стокер",
                    Description = "Классическая история о самом известном вампире",
                    Price = 300, Image = $"/images/Dracula.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("horror"))
                },
                new Book
                {
                    Title = "Мастер и Маргарита",
                    Author = "Михаил Булгаков",
                    Description = "Визит дьявола в Москву 1930-х годов",
                    Price = 500, Image = $"/images/Master-and-Margarita.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("mysticism"))
                }
            },
            TotalPages = 10
        };

        ResponseData<ListModel<Book>> bookList = new ResponseData<ListModel<Book>>
        {
            Data = expectedBooks,
            ErrorMessage = "Unluck bookList",
            Successfull = true
        };

        bookCategoryServ.GetCategoryListAsync().Returns(categoryList);
        bookServ.GetBookListAsync(category, pageNo).Returns(bookList);
        var controller = new BookController(bookCategoryServ, bookServ);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await controller.Index(category, pageNo);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var viewModel = Assert.IsType<BookListViewModel>(viewResult.Model);
        Assert.Same(expectedBooks, viewModel.Books);
    }

    [Fact]
    public async Task Index_CurrentCategory_ShouldBePassedToViewData()
    {
        // Arrange
        var bookCategoryServ = Substitute.For<IBookCategoryService>();
        var bookServ = Substitute.For<IBookService>();

        const string category = "adventure";
        const int pageNo = 2;

        var expectedCategories = new List<Category>
        {
            new Category { Name = "Фантастика", NormalizedName = "fantasy" },
            new Category { Name = "Детектив", NormalizedName = "detective" },
            new Category { Name = "Приключение", NormalizedName = "adventure" },
            new Category { Name = "Романтика", NormalizedName = "romance" },
            new Category { Name = "Ужасы", NormalizedName = "horror" },
            new Category { Name = "Мистика", NormalizedName = "mysticism" }
        };

        ResponseData<List<Category>> categoryList = new ResponseData<List<Category>>
        {
            Data = expectedCategories,
            ErrorMessage = "Unluck bookCategoryList",
            Successfull = true
        };

        var expectedBooks = new ListModel<Book>
        {
            CurrentPage = pageNo,
            Items = new List<Book>
            {
                new Book
                {
                    Title = "Автостопом по галактике",
                    Description = "Комическое путешествие по космосу после гибели Земли",
                    Author = "Дуглас Адамс",
                    Price = 100, Image = $"/images/b7c0ee8b-97a4-4928-bfc2-ae7a7ea9c5e5.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("fantasy"))
                },
                new Book
                {
                    Title = "Приключения Шерлока Холмса",
                    Author = "Артур Конан Дойл",
                    Description = "Гениальный сыщик решает запутанные головоломки",
                    Price = 150, Image = $"/images/ba069570-fe3a-47d8-b6eb-98bf259707c2.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("detective"))
                },
                new Book
                {
                    Title = "Остров Сокровищ",
                    Author = "Роберт Льюис Стивенсон",
                    Description = "Поиски пиратского клада, полные опасностей",
                    Price = 120, Image = $"/images/Treasure-Island.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("adventure"))
                },
                new Book
                {
                    Title = "Анна Каренина",
                    Author = "Лев Толстой",
                    Description = "Трагическая любовь замужней дамы в высшем свете",
                    Price = 200, Image = $"/images/Anna-Carenina.png",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("romance"))
                },
                new Book
                {
                    Title = "Дракула",
                    Author = "Брэм Стокер",
                    Description = "Классическая история о самом известном вампире",
                    Price = 300, Image = $"/images/Dracula.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("horror"))
                },
                new Book
                {
                    Title = "Мастер и Маргарита",
                    Author = "Михаил Булгаков",
                    Description = "Визит дьявола в Москву 1930-х годов",
                    Price = 500, Image = $"/images/Master-and-Margarita.jpeg",
                    Category = expectedCategories.Find(c => c.NormalizedName.Equals("mysticism"))
                }
            },
            TotalPages = 10
        };

        ResponseData<ListModel<Book>> bookList = new ResponseData<ListModel<Book>>
        {
            Data = expectedBooks,
            ErrorMessage = "Unluck bookList",
            Successfull = true
        };

        bookCategoryServ.GetCategoryListAsync().Returns(categoryList);
        bookServ.GetBookListAsync(category, pageNo).Returns(bookList);
        var controller = new BookController(bookCategoryServ, bookServ);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await controller.Index(category, pageNo);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.True(viewResult.ViewData.ContainsKey("currentCategory"));
        Assert.Equal(category, viewResult.ViewData["currentCategory"]);
    }
}