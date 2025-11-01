using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.UI.Extensions;

namespace WEB_353503_Sebelev.UI.Services.CartService;

public class SessionCart : Cart
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionCart(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        
        var sessionCart = _httpContextAccessor.HttpContext?.Session.Get<Cart>("cart");
        if (sessionCart is not null)
        {
            CartItems = sessionCart.CartItems;
        }
    }

    public override void AddToCart(Book book)
    {
        base.AddToCart(book);
        _httpContextAccessor.HttpContext?.Session.Set("cart", this);
    }

    public override void RemoveItem(int id)
    {
        base.RemoveItem(id);
        _httpContextAccessor.HttpContext?.Session.Set("cart", this);
    }

    public override void ClearCart()
    {
        base.ClearCart();
        _httpContextAccessor.HttpContext?.Session.Set("cart", this);
    }
}