namespace WEB_353503_Sebelev.UI.Services.FileService;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile? file);
    Task DeleteFileAsync(string fileName);
}