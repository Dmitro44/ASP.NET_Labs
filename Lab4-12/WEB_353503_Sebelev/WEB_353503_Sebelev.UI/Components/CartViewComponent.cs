using Microsoft.AspNetCore.Mvc;
using WEB_353503_Sebelev.UI.Extensions;

namespace WEB_353503_Sebelev.UI.Components;

public class CartViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        Domain.Entities.Cart cart = HttpContext.Session.Get<Domain.Entities.Cart>("cart");

        CartInfo cartInfo = new CartInfo
        {
            ItemCount = 0,
            TotalPrice = 0.00m
        };

        if (cart is not null)
        {
            cartInfo.ItemCount = cart.TotalCount;
            cartInfo.TotalPrice = cart.TotalAmount;
        }
        
        return View(cartInfo);
    }
}

public class CartInfo
{
    public int ItemCount { get; set; }
    public decimal TotalPrice { get; set; }
}