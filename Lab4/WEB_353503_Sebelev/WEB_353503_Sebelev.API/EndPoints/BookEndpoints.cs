using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WEB_353503_Sebelev.API.Data;
using WEB_353503_Sebelev.API.UseCases;
using WEB_353503_Sebelev.Domain.Entities;
namespace WEB_353503_Sebelev.API.EndPoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Book");

        group.MapGet("/{category:alpha?}", async (IMediator mediator, string? category, int pageNo = 1) =>
            {
                var data = await mediator.Send(new GetListOfBooks(category, pageNo));
                return TypedResults.Ok(data);
            })
        .WithName("GetAllBooks");

        group.MapGet("/{id}", async Task<Results<Ok<Book>, NotFound>> (int id, AppDbContext db) =>
        {
            return await db.Books.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Book model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetBookById");

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int id, Book book, AppDbContext db) =>
        {
            var affected = await db.Books
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, book.Id)
                    .SetProperty(m => m.Title, book.Title)
                    .SetProperty(m => m.Description, book.Description)
                    .SetProperty(m => m.Author, book.Author)
                    .SetProperty(m => m.CategoryId, book.CategoryId)
                    .SetProperty(m => m.Price, book.Price)
                    .SetProperty(m => m.Image, book.Image)
                    .SetProperty(m => m.MimePath, book.MimePath)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateBook");

        group.MapPost("/", async (Book book, AppDbContext db) =>
        {
            db.Books.Add(book);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Book/{book.Id}",book);
        })
        .WithName("CreateBook");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db) =>
        {
            var affected = await db.Books
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteBook");
    }
}
