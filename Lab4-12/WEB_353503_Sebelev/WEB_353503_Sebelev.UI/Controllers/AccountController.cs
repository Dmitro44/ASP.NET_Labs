using Microsoft.AspNetCore.Mvc;

namespace WEB_353503_Sebelev.UI.Controllers;

public class AccountController : Controller
{
   public IActionResult LogOut()
   {
      return View();
   }
}