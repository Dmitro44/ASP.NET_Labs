using MediatR;
using Microsoft.AspNetCore.Mvc;
using WEB_353503_Sebelev.API.UseCases;

namespace WEB_353503_Sebelev.API.EndPoints;

public static class FileEndpoints
{
    public static void MapFileEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/File")
            .DisableAntiforgery()
            .RequireAuthorization();

        group.MapPost("/", async (IFormFile? file, IMediator mediator) =>
        {
            try
            {
                var imagePath = await mediator.Send(new SaveImage(file));
                return Results.Ok(imagePath);
            }
            catch
            {
                return Results.BadRequest();
            }
        });

        group.MapDelete("/", async (IMediator mediator, [FromQuery] string imagePath) =>
        {
            try
            {
                await mediator.Send(new DeleteImage(imagePath));
                return Results.Ok();
            }
            catch
            {
                return Results.BadRequest();
            }
        });
    }
}