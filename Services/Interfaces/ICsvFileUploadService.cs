namespace FarmManagerAPI.Services.Interfaces
{
    public interface ICsvFileUploadService
    {
        Task ReadFileContent(IFormFile file, Guid farmId);
    }
}
