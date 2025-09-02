namespace WEB_353503_Sebelev.Domain.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Category? Category { get; set; }
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public string? ImagePath { get; set; }
    public string? ImageMimePath { get; set; }
}