using WEB_353503_Sebelev.UI.Services.BookCategoryService;

namespace WEB_353503_Sebelev.UI.Extensions;

public static class HostingExtensions
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IBookCategoryService, MemoryBookCategoryService>();
    }
}