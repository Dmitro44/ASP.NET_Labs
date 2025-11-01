namespace WEB_353503_Sebelev.Domain.Entities;

public class CartItem
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string? Image { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
}