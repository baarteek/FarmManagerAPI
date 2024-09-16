namespace FarmManagerAPI.Services.Interfaces;

public interface IFileUploadService
{
    Task<string> ReadFileContentAsync(IFormFile file);
}