using Microsoft.AspNetCore.Mvc;

namespace WEB_353503_Sebelev.UI.Controllers;

[Area("Admin")]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}