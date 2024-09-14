using System.Text;
using FarmManagerAPI.Services.Interfaces;

namespace FarmManagerAPI.Services.Implementations;

public class FileUploadService : IFileUploadService
{
    private readonly string _targetFilePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

    public FileUploadService()
    {
        if (!Directory.Exists(_targetFilePath))
        {
            Directory.CreateDirectory(_targetFilePath);
        }
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file provided or the file is empty.");

        if (Path.GetExtension(file.FileName).ToLower() != ".gml")
            throw new ArgumentException("Only .gml files are allowed.");

        var filePath = Path.Combine(_targetFilePath, file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return filePath;
    }

    public async Task<string> ReadFileContentAsync(string filePath)
    {
        return await File.ReadAllTextAsync(filePath, Encoding.UTF8);
    }
}