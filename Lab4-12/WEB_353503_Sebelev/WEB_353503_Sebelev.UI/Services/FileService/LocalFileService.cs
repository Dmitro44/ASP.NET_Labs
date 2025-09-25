namespace WEB_353503_Sebelev.UI.Services.FileService;

public class LocalFileService(IWebHostEnvironment env) : IFileService
{
    private const string ImageFolderName = "images";
    
    public async Task<string> SaveFileAsync(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}-avatar{extension}";
        
        var directoryPath = Path.Combine(env.WebRootPath, ImageFolderName);
        var filePath = Path.Combine(directoryPath, fileName);

        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return Path.Combine(ImageFolderName, fileName);
    }
}