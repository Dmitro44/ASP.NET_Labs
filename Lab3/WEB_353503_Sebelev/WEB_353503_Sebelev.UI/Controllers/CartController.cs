using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WEB_353503_Sebelev.UI.Controllers;

public class CartController : Controller
{
   public IActionResult Index()
   {
      return View();
   }

   public IActionResult Add()
   {
      return View();
   }
}