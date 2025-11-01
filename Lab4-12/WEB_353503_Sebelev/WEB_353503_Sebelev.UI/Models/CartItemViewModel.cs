namespace WEB_353503_Sebelev.UI.Models;

public class CartItemViewModel
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Count { get; set; }
    public string? Image { get; set; }
}