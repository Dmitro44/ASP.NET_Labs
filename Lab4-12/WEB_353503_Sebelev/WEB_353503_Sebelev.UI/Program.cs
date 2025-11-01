using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using WEB_353503_Sebelev.UI.Extensions;
using WEB_353503_Sebelev.UI.HelperClasses;
using WEB_353503_Sebelev.UI.Services.BookCategoryService;
using WEB_353503_Sebelev.UI.Services.BookService;
using WEB_353503_Sebelev.UI.Services.FileService;

namespace WEB_353503_Sebelev.UI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddSession();
        builder.Services.AddDistributedMemoryCache();
        
        builder.Services.AddHttpContextAccessor();
        builder.RegisterCustomServices(); 
        
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

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
        
        builder.Services.AddHttpClient<IFileService, ApiFileService>(opt
                => opt.BaseAddress = new Uri(apiUri + "File"))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            });
        
        var keycloakData = builder.Configuration
            .GetSection("Keycloak")
            .Get<KeycloakData>();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "keycloak";
            })
            .AddCookie()
            .AddOpenIdConnect("keycloak", options =>
            {
                options.Authority = $"{keycloakData.Host}/auth/realms/{keycloakData.Realm}";
                options.ClientId = keycloakData.ClientId;
                options.ClientSecret = keycloakData.ClientSecret;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.Scope.Add("openid");
                options.SaveTokens = true;
                options.RequireHttpsMetadata = false;
                options.MetadataAddress =
                    $"{keycloakData.Host}/realms/{keycloakData.Realm}/.well-known/openid-configuration";
            });
        
        builder.Services
            .AddAuthorization(opt =>
            {
                opt.AddPolicy("admin",p => p.RequireRole("POWER-USER"));
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.MapRazorPages().RequireAuthorization("admin");

        app.UseSession();

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
