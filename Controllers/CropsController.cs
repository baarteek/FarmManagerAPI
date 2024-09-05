using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CropsController : ControllerBase
    {
        private readonly ICropService _cropService;

        public CropsController(ICropService cropService)
        {
            _cropService = cropService;
        }

        [HttpPost]
        public async Task<ActionResult<CropDTO>> AddCrop(CropEditDTO cropEditDto)
        {
            var crop = await _cropService.AddCrop(cropEditDto);
            return CreatedAtAction(nameof(GetCropById), new { id = crop.Id }, crop);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CropDTO>> GetCropById(Guid id)
        {
            try
            {
                var crop = await _cropService.GetCropById(id);
                return Ok(crop);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("user/active")]
        public async Task<ActionResult<IEnumerable<CropDTO>>> GetActiveCropsByUser()
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userName == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            try
            {
                var crops = await _cropService.GetActiveCropsByUser(userName);
                return Ok(crops);
            }
            catch(Exception ex)
            {
                return NotFound(new { message = ex.Message});
            }
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<CropDTO>>> GetCropsByUser()
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userName == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            try
            {
                var crops = await _cropService.GetCropsByUser(userName);
                return Ok(crops);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("field/{fieldId}")]
        public async Task<ActionResult<IEnumerable<CropDTO>>> GetCropsByFieldId(Guid fieldId)
        {
            var crops = await _cropService.GetCropsByFieldId(fieldId);
            return Ok(crops);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCrop(Guid id, CropEditDTO cropEditDto)
        {
            try
            {
                await _cropService.UdpateCrop(id, cropEditDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCrop(Guid id)
        {
            try
            {
                await _cropService.DeleteCrop(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("/Crops/cropType")]
        public IActionResult GetCropTypes()
        {
            var values = Enum.GetValues(typeof(CropType))
                .Cast<CropType>()
                .Select(e => new EnumResponse { Id = (int)e, Name = e.ToString() })
                .ToList();

            return Ok(values);
        }
    }
}
