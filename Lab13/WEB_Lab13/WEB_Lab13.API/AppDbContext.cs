using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WEB_Lab13.Core.Entities;

namespace WEB_Lab13.API;

public class AppDbContext : IdentityDbContext<User> 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
}