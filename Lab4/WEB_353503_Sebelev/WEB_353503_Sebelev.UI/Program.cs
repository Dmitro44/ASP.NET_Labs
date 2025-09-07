using Microsoft.Extensions.Options;
using WEB_353503_Sebelev.UI.Extensions;
using WEB_353503_Sebelev.UI.Services.BookCategoryService;
using WEB_353503_Sebelev.UI.Services.BookService;

namespace WEB_353503_Sebelev.UI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.RegisterCustomServices();

        builder.Services.Configure<UriData>(builder.Configuration.GetSection("UriData"));

        var provider = builder.Services.BuildServiceProvider();
        var uriData = provider.GetRequiredService<IOptions<UriData>>().Value;
        var apiUri = uriData.ApiUri;
        if (!apiUri.EndsWith("/")) apiUri += "/";

        builder.Services.AddHttpClient<IBookCategoryService, ApiBookCategoryService>(opt
                => opt.BaseAddress = new Uri(apiUri + "Categories"))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            });
        builder.Services.AddHttpClient<IBookService, ApiBookService>(opt
                => opt.BaseAddress = new Uri(apiUri + "Book"))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
