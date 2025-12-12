using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using WEB_353503_Sebelev.API.Data;
using WEB_353503_Sebelev.API.EndPoints;
using WEB_353503_Sebelev.API.Models;
using WEB_353503_Sebelev.API.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options. AddPolicy("AllowBlazor", policy =>
    {
        policy
            .WithOrigins("https://localhost:7099")
            .AllowAnyMethod()
            .AllowAnyHeader();
        // .AllowCredentials();
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetListOfBooksHandler).Assembly));

var authServer = builder.Configuration
    .GetSection("AuthServer")
    .Get<AuthServerData>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
    {
        o.MetadataAddress = $"{authServer.Host}/realms/{authServer.Realm}/.well-known/openid-configuration";

        o.Authority = $"{authServer.Host}/realms/{authServer.Realm}";

        o.Audience = "account";

        o.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("admin", p => p.RequireRole("POWER-USER"));
});

builder.Services.AddHybridCache();
builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.InstanceName = "labs_";
    opt.Configuration = builder.Configuration.GetConnectionString("Redis");
});

var app = builder.Build();

await DbInitializer.SeedData(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowBlazor");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapBookEndpoints();
app.MapFileEndpoints();

app.Run();