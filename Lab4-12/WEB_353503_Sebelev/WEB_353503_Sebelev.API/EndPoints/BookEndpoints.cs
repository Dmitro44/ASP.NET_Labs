using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WEB_353503_Sebelev.API.Data;
using WEB_353503_Sebelev.API.UseCases;
using WEB_353503_Sebelev.Domain.Entities;
using WEB_353503_Sebelev.Domain.Models;

namespace WEB_353503_Sebelev.API.EndPoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Book")
            .WithTags(nameof(Book))
            .DisableAntiforgery();

        group.MapGet("/{category:alpha?}", async (IMediator mediator, string? category, int pageNo = 1) =>
        {
            var data = await mediator.Send(new GetListOfBooks(category, pageNo));
            return TypedResults.Ok(data);
        })
        .WithName("GetAllBooks");

        group.MapGet("/{id}", async Task<Results<Ok<ResponseData<Book>>, NotFound<ResponseData<Book>>>> (int id, AppDbContext db) =>
        {
            var book = await db.Books.AsNoTracking()
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book is not null)
            {
                return TypedResults.Ok(ResponseData<Book>.Success(book));
            }

            return TypedResults.NotFound(ResponseData<Book>.Error($"Книга с ID {id} не найдена", book));
        })
        .WithName("GetBookById");

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (
                int id,
                [FromForm] string book,
                [FromForm] IFormFile? file,
                AppDbContext db,
                IMediator mediator) =>
        {
            var existingBook = await db.Books.FindAsync(id);
            if (existingBook is null)
            {
                return TypedResults.NotFound();
            }

            var oldImagePath = existingBook.Image;

            var updatedBook = JsonSerializer.Deserialize<Book>(book);
            updatedBook.Image = oldImagePath;

            if (file is not null)
            {
                var newImagePath = await mediator.Send(new SaveImage(file));
                updatedBook.Image = newImagePath;
                
                await mediator.Send(new DeleteImage(oldImagePath));
            }
            
            var affected = await db.Books
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, updatedBook.Id)
                    .SetProperty(m => m.Title, updatedBook.Title)
                    .SetProperty(m => m.Description, updatedBook.Description)
                    .SetProperty(m => m.Author, updatedBook.Author)
                    .SetProperty(m => m.CategoryId, updatedBook.CategoryId)
                    .SetProperty(m => m.Price, updatedBook.Price)
                    .SetProperty(m => m.Image, updatedBook.Image)
                    .SetProperty(m => m.MimePath, updatedBook.MimePath)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateBook");

        group.MapPost("/", async (
                [FromForm] string book,
                [FromForm] IFormFile? file,
                AppDbContext db,
                IMediator mediator) => 
        {
            var newBook = JsonSerializer.Deserialize<Book>(book);
            if (file is not null)
            {
                newBook.Image = await mediator.Send(new SaveImage(file));
            }
        
            db.Books.Add(newBook);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Book/{newBook.Id}", newBook);
        })
        .WithName("CreateBook");

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int id, AppDbContext db, IMediator mediator) =>
        {
            var book = await db.Books.FindAsync(id);

            if (book is null)
            {
                return TypedResults.NotFound();
            }
            
            var imagePathToDelete = book.Image;
            
            db.Books.Remove(book);
            await db.SaveChangesAsync();

            if (!string.IsNullOrEmpty(imagePathToDelete))
            {
                await mediator.Send(new DeleteImage(imagePathToDelete));
            }
            
            return TypedResults.Ok();
        })
        .WithName("DeleteBook");
    }
}
