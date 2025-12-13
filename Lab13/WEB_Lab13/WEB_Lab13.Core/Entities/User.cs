using Microsoft.AspNetCore.Identity;

namespace WEB_Lab13.Core.Entities;

public class User : IdentityUser
{
    public string? DisplayName { get; set; }
}