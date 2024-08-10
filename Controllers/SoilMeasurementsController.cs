using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmManagerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SoilMeasurementsController : ControllerBase
    {
        private readonly ISoilMeasurementService _soilMeasurementService;

        public SoilMeasurementsController(ISoilMeasurementService soilMeasurementService)
        {
            _soilMeasurementService = soilMeasurementService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SoilMeasurementDTO>> GetSoilMeasurementById(Guid id)
        {
            try
            {
                var soilMeasurement = await _soilMeasurementService.GetSoilMeasurementById(id);
                return Ok(soilMeasurement);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("field/{fieldId}")]
        public async Task<ActionResult<IEnumerable<SoilMeasurementDTO>>> GetSoilMeasurementsByFieldId(Guid fieldId)
        {
            var soilMeasurements = await _soilMeasurementService.GetSoilMeasurementsByFieldId(fieldId);
            return Ok(soilMeasurements);
        }

        [HttpPost]
        public async Task<ActionResult<SoilMeasurementDTO>> AddSoilMeasurement(SoilMeasurementEditDTO soilMeasurementEditDto)
        {
            try
            {
                var soilMeasurement = await _soilMeasurementService.AddSoilMeasurement(soilMeasurementEditDto);
                return CreatedAtAction(nameof(GetSoilMeasurementById), new { id = soilMeasurement.Id }, soilMeasurement);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SoilMeasurementDTO>> UpdateSoilMeasurement(Guid id, SoilMeasurementEditDTO soilMeasurementEditDto)
        {
            try
            {
                var soilMeasurement = await _soilMeasurementService.UpdateSoilMeasurement(id, soilMeasurementEditDto);
                if (soilMeasurement == null)
                {
                    return NotFound();
                }
                return Ok(soilMeasurement);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSoilMeasurement(Guid id)
        {
            try
            {
                await _soilMeasurementService.DeleteSoilMeasurement(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
