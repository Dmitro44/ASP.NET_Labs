using Microsoft.EntityFrameworkCore;
using WEB_353503_Sebelev.Domain.Entities;

namespace WEB_353503_Sebelev.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
}