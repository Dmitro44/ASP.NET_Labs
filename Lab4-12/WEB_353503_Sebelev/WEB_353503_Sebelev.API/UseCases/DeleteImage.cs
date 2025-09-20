using MediatR;

namespace WEB_353503_Sebelev.API.UseCases;

public sealed record DeleteImage(string ImagePath) : IRequest;

public class DeleteImageHandler(IWebHostEnvironment env) : IRequestHandler<DeleteImage>
{
    public Task Handle(DeleteImage request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.ImagePath))
        {
            return Task.CompletedTask;
        }
        
        var pathComponent = Uri.TryCreate(request.ImagePath, UriKind.Absolute, out var path)
            ? path.AbsolutePath
            : request.ImagePath;
        
        var relativePath = pathComponent.TrimStart('/');
        var serverPath = Path.Combine(env.WebRootPath, relativePath);

        serverPath = serverPath.Replace('/', Path.DirectorySeparatorChar);

        if (File.Exists(serverPath))
        {
            File.Delete(serverPath);
        }
        
        return Task.CompletedTask;
    }
}