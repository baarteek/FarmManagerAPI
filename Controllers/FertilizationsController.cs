﻿using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FertilizationsController : ControllerBase
    {
        private readonly IFertilizationService _fertilizationService;

        public FertilizationsController(IFertilizationService fertilizationService)
        {
            _fertilizationService = fertilizationService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FertilizationDTO>> GetFertilization(Guid id)
        {
            try
            {
                var fertilization = await _fertilizationService.GetFertilizationById(id);
                return Ok(fertilization);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("crop/{cropId}")]
        public async Task<ActionResult<IEnumerable<FertilizationDTO>>> GetFertilizationsByCropId(Guid cropId)
        {
            var fertilizations = await _fertilizationService.GetFertilizationsByCropId(cropId);
            return Ok(fertilizations);
        }

        [HttpPost]
        public async Task<ActionResult<FertilizationDTO>> AddFertilization(FertilizationEditDTO fertilizationEditDto)
        {
            try
            {
                var fertilization = await _fertilizationService.AddFertilization(fertilizationEditDto);
                return CreatedAtAction(nameof(GetFertilization), new { id = fertilization.Id }, fertilization);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FertilizationDTO>> UpdateFertilization(Guid id, FertilizationEditDTO fertilizationEditDto)
        {
            try
            {
                var fertilization = await _fertilizationService.UpdateFertilization(id, fertilizationEditDto);
                return Ok(fertilization);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFertilization(Guid id)
        {
            await _fertilizationService.DeleteFertilization(id);
            return NoContent();
        }

        [HttpGet("/Fertilizations/fertilizationType")]
        public IActionResult GetFertilizationTypes()
        {
            var values = Enum.GetValues(typeof(FertilizationType))
                .Cast<FertilizationType>()
                .Select(e => new EnumResponse { Id = (int)e, Name = e.ToString() });

            return Ok(values);
        }
    }
}
