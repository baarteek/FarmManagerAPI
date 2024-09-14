namespace FarmManagerAPI.Services.Interfaces;

public interface IFileUploadService
{
    Task<string> SaveFileAsync(IFormFile file);
    Task<string> ReadFileContentAsync(string filePath);
}