namespace WEB_353503_Sebelev.UI.Models;

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}