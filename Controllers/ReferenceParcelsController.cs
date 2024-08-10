using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ReferenceParcelsController : ControllerBase
    {
        private readonly IReferenceParcelService _parcelService;

        public ReferenceParcelsController(IReferenceParcelService parcelService)
        {
            _parcelService = parcelService;
        }

        [HttpPost]
        public async Task<ActionResult<ReferenceParcelDTO>> AddParcel(ReferenceParcelEditDTO parcelEditDto)
        {
            var parcel = await _parcelService.AddParcel(parcelEditDto);
            return CreatedAtAction(nameof(GetParcelById), new { id = parcel.Id }, parcel);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReferenceParcelDTO>> GetParcelById(Guid id)
        {
            try
            {
                var parcel = await _parcelService.GetParcelById(id);
                return Ok(parcel);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("field/{fieldId}")]
        public async Task<ActionResult<IEnumerable<ReferenceParcelDTO>>> GetParcelsByFieldId(Guid fieldId)
        {
            var parcels = await _parcelService.GetParcelsByFieldId(fieldId);
            return Ok(parcels);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateParcel(Guid id, ReferenceParcelEditDTO parcelEditDto)
        {
            try
            {
                await _parcelService.UpdateParcel(id, parcelEditDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteParcel(Guid id)
        {
            try
            {
                await _parcelService.DeleteParcel(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
