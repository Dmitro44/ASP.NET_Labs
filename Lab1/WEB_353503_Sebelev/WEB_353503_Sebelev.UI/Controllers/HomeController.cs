using Microsoft.AspNetCore.Mvc;

namespace WEB_353503_Sebelev.UI.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    
}