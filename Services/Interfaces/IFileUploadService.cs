namespace FarmManagerAPI.Services.Interfaces;

public interface IFileUploadService
{
    Task ReadFileContent(IFormFile file, Guid farmId);
}