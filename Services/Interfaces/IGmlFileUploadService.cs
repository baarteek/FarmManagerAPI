namespace FarmManagerAPI.Services.Interfaces;

public interface IGmlFileUploadService
{
    Task ReadFileContent(IFormFile file, Guid farmId);
}