using Microsoft.AspNetCore.Mvc;

namespace WEB_353503_Sebelev.UI.Components;

public class Cart : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var cartInfo = new CartInfo
        {
            ItemCount = 0,
            TotalPrice = 0.00,
        };
        
        return View(cartInfo);
    }
}

public class CartInfo
{
    public int ItemCount { get; set; }
    public double TotalPrice { get; set; }
}