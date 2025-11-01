using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.UI.Models;
using WEB_353503_Sebelev.UI.Services.BookService;

namespace WEB_353503_Sebelev.UI.Controllers;

public class CartController : Controller
{
   private readonly IBookService _apiBookService;
   private readonly Cart _cart;

   public CartController(IBookService apiBookService, Cart cart)
   {
      _apiBookService = apiBookService;
      _cart = cart;
   }
   
   [Authorize]
   public IActionResult Index()
   {
      var cartViewModel = new CartViewModel
      {
         Items = _cart.CartItems.Select(item => new CartItemViewModel
            {
               BookId = item.Key,
               Title = item.Value.Title,
               Description = item.Value.Description,
               Count = item.Value.Count,
               Price = item.Value.Price,
               Image = item.Value.Image
            })
            .ToList(),
         TotalAmount = _cart.TotalAmount,
      };
      
      return View(cartViewModel);
   }

   [Authorize]
   [Route("[controller]/add/{id:int}")]
   public async Task<IActionResult> Add(int id, string returnUrl)
   {
      var data = await _apiBookService.GetBookByIdAsync(id);
      if (data.Successfull)
      {
         _cart.AddToCart(data.Data);
      }
      return Redirect(returnUrl);
   }

   [Authorize]
   [Route("[controller]/remove/{id:int}")]
   public IActionResult Remove(int id, string returnUrl = "/Cart")
   {
      _cart.RemoveItem(id);
      return Redirect(returnUrl);
   }

   [Authorize]
   [Route("[controller]/clear")]
   public IActionResult Clear(string returnUrl = "/Cart")
   {
      _cart.ClearCart();
      return Redirect(returnUrl);
   }
}