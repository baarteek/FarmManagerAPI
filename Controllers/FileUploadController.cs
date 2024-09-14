using Microsoft.AspNetCore.Authorization;

namespace FarmManagerAPI.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Services.Interfaces;

[Authorize]
[Route("[controller]")]
[ApiController]
public class FileUploadController : ControllerBase
{
    private readonly IFileUploadService _fileUploadService;

    public FileUploadController(IFileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }

    [HttpPost("uploadGMLFile")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided or the file is empty.");
        }

        try
        {
            var filePath = await _fileUploadService.SaveFileAsync(file);
            var fileContent = await _fileUploadService.ReadFileContentAsync(filePath);
            
            Console.WriteLine($"Content of {file.FileName}:\n{fileContent}");

            return Ok(new { message = "File uploaded successfully", fileName = file.FileName });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}