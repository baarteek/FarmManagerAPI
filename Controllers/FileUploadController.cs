using Microsoft.AspNetCore.Authorization;

namespace FarmManagerAPI.Controllers
{
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
        private readonly IGmlFileUploadService _gmFlileUploadService;
        private readonly ICsvFileUploadService _csvFileUploadService;

        public FileUploadController(IGmlFileUploadService gmlFileUploadService, ICsvFileUploadService csvFileUploadService)
        {
            _gmFlileUploadService = gmlFileUploadService;
            _csvFileUploadService = csvFileUploadService;
        }

        [HttpPost("uploadGMLFile/{farmId}")]
        public async Task<IActionResult> UploadGmlFile(IFormFile file, Guid farmId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided or the file is empty.");
            }

            try
            { 
                await _gmFlileUploadService.ReadFileContent(file, farmId);
                return Ok(new { message = "File uploaded and processed successfully", fileName = file.FileName });
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

        [HttpPost("uploadCSVFile/{farmId}")]
        public async Task<IActionResult> UploadCsvFile(IFormFile file, Guid farmId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided or the file is empty.");
            }

            try
            {
                await _csvFileUploadService.ReadFileContent(file, farmId);
                return Ok(new { message = "File uploaded and processed successfully", fileName = file.FileName });
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
}