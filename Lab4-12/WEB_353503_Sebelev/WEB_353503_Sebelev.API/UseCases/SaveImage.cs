using MediatR;

namespace WEB_353503_Sebelev.API.UseCases;

public sealed record SaveImage(IFormFile? File) : IRequest<string>;

public class SaveImageHandler(
    IWebHostEnvironment env,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<SaveImage, string>
{
    private const string ImageFolderName = "images";
    
    public async Task<string> Handle(SaveImage request, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(request.File.FileName);
        var newImageFileName = $"{Guid.NewGuid()}{extension}";

        var imageDirectoryPath = Path.Combine(env.WebRootPath, ImageFolderName);
        var imagePath = Path.Combine(imageDirectoryPath, newImageFileName);
        
        Directory.CreateDirectory(imageDirectoryPath);
        
        using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        var httpContextRequest = httpContextAccessor.HttpContext?.Request;

        var relativeUrl = $"/{ImageFolderName}/{newImageFileName}";
        var absoluteUrl = $"{httpContextRequest.Scheme}://{httpContextRequest.Host}{relativeUrl}";
        
        return absoluteUrl;
    } 
}