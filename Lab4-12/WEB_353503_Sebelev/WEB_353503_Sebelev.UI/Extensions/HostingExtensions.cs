using WEB_353503_Sebelev.UI.HelperClasses;
using WEB_353503_Sebelev.UI.Services.Authentication;
using WEB_353503_Sebelev.UI.Services.BookCategoryService;
using WEB_353503_Sebelev.UI.Services.BookService;
using WEB_353503_Sebelev.UI.Services.FileService;

namespace WEB_353503_Sebelev.UI.Extensions;

public static class HostingExtensions
{
    public static void RegisterCustomServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IBookCategoryService, ApiBookCategoryService>();
        builder.Services.AddScoped<IBookService, ApiBookService>();
        
        builder.Services
            .Configure<KeycloakData>(builder.Configuration.GetSection("Keycloak"));
        builder.Services.AddHttpClient<ITokenAccessor, KeycloakTokenAccessor>();

        builder.Services.AddScoped<KeycloakAuthService>();
        builder.Services.AddScoped<IFileService, ApiFileService>();
    }
}