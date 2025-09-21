namespace WEB_353503_Sebelev.Domain.Entities;

public class Book
{
    public int? Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    private Category? _category;

    public Category? Category
    {
        get => _category;
        set
        {
            _category = value;
            CategoryId = _category?.Id ?? 0;
        }
    }
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public string? Image { get; set; }
    public string? MimePath { get; set; }
}