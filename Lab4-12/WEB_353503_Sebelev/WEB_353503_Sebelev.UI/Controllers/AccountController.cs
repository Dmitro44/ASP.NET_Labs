using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WEB_353503_Sebelev.UI.Models;
using WEB_353503_Sebelev.UI.Services.Authentication;

namespace WEB_353503_Sebelev.UI.Controllers;

public class AccountController(KeycloakAuthService service) : Controller
{
   [HttpGet]
   public IActionResult Register()
   {
      return View(new RegisterUserViewModel());
   }

   [HttpPost]
   public async Task<IActionResult> Register(RegisterUserViewModel model)
   {
      if (ModelState.IsValid)
      {
         if (model is null)
         {
            return BadRequest();
         }

         var result = await service.RegisterUserAsync(model.Email, model.Password, model.Avatar);

         if (result.Result)
         {
            return RedirectToAction("Index", "Home");
         }

         ModelState.AddModelError("RegisterError", result.Message);
      }

      return View(model);
   }

   public async Task Login()
   {
      await HttpContext.ChallengeAsync(
         "keycloak", 
         new AuthenticationProperties { RedirectUri = Url.Action("Index", "Home") });
   }
   
   [HttpPost]
   public async Task LogOut()
   {
      await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
      await HttpContext.SignOutAsync("keycloak",
         new AuthenticationProperties { RedirectUri = Url.Action("Index", "Home") });
      
      HttpContext.Session.Clear();
   }
}