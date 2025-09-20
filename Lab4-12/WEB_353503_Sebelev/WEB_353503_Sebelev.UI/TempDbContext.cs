using Microsoft.EntityFrameworkCore;
using WEB_353503_Sebelev.Domain.Entities;

namespace WEB_353503_Sebelev.UI;

public class TempDbContext : DbContext
{
   public DbSet<Book> Books { get; set; }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   {
      base.OnConfiguring(optionsBuilder);
      optionsBuilder.UseSqlite("");
   }
}