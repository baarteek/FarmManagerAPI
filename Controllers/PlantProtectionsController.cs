using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PlantProtectionsController : ControllerBase
    {
        private readonly IPlantProtectionService _plantProtectionService;

        public PlantProtectionsController(IPlantProtectionService plantProtectionService)
        {
            _plantProtectionService = plantProtectionService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlantProtectionDTO>> GetPlantProtection(Guid id)
        {
            try
            {
                var plantProtection = await _plantProtectionService.GetPlantProtectionById(id);
                return Ok(plantProtection);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("crop/{cropId}")]
        public async Task<ActionResult<IEnumerable<PlantProtectionDTO>>> GetPlantProtectionsByCropId(Guid cropId)
        {
            var plantProtections = await _plantProtectionService.GetPlantProtectionsByCropId(cropId);
            return Ok(plantProtections);
        }

        [HttpPost]
        public async Task<ActionResult<PlantProtectionDTO>> AddPlantProtection(PlantProtectionEditDTO plantProtectionEditDto)
        {
            try
            {
                var plantProtection = await _plantProtectionService.AddPlantProtection(plantProtectionEditDto);
                return CreatedAtAction(nameof(GetPlantProtection), new { id = plantProtection.Id }, plantProtection);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PlantProtectionDTO>> UpdatePlantProtection(Guid id, PlantProtectionEditDTO plantProtectionEditDto)
        {
            try
            {
                var plantProtection = await _plantProtectionService.UpdatePlantProtection(id, plantProtectionEditDto);
                return Ok(plantProtection);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePlantProtection(Guid id)
        {
            await _plantProtectionService.DeletePlantProtection(id);
            return NoContent();
        }

        [HttpGet("/PlantProtections/plantProtectionType")]
        public IActionResult GetPlantProtectionTypes()
        {
            var values = Enum.GetValues(typeof(PlantProtectionType))
                .Cast<PlantProtectionType>()
                .Select(e => new EnumResponse { Id = (int)e, Name = e.ToString() })
                .ToList();

            return Ok(values);
        }
    }
}
