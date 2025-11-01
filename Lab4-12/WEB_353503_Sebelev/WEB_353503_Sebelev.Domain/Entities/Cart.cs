namespace WEB_353503_Sebelev.Domain.Entities;

public class Cart
{
    public Dictionary<int, CartItem> CartItems { get; set; } = new();

    public virtual void AddToCart(Book book)
    {
        if (CartItems.TryGetValue(book.Id, out var item))
        {
            item.Count++;
        }
        else
        {
            CartItems.Add(book.Id, new CartItem
            {
                Title = book.Title,
                Description = book.Description,
                Price = book.Price,
                Image = book.Image,
                Count = 1
            });    
        }
    }

    public virtual void RemoveItem(int id) => CartItems.Remove(id);

    public virtual void ClearCart() => CartItems.Clear();

    public int TotalCount => CartItems.Sum(item => item.Value.Count);

    public decimal TotalAmount => CartItems.Sum(item => item.Value.Price * item.Value.Count);
}