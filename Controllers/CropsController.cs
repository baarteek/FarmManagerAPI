using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
