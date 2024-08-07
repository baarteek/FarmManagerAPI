using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmManagerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FieldsController : ControllerBase
    {
        private readonly IFieldService _fieldService;

        public FieldsController(IFieldService fieldService)
        {
            _fieldService = fieldService;
        }

        [HttpPost]
        public async Task<ActionResult<FieldDTO>> AddField(FieldEditDTO fieldEditDto)
        {
            try
            {
                var field = await _fieldService.AddField(fieldEditDto);
                return CreatedAtAction(nameof(GetFieldById), new { id = field.Id }, field);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FieldDTO>> GetFieldById(Guid id)
        {
            try
            {
                var field = await _fieldService.GetFieldById(id);
                if (field == null)
                {
                    return NotFound(new { message = "Field not found" });
                }
                return Ok(field);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("byFarm/{farmId}")]
        public async Task<ActionResult<IEnumerable<FieldDTO>>> GetFieldsByFarmId(Guid farmId)
        {
            var fields = await _fieldService.GetFieldsByFarmId(farmId);
            return Ok(fields);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateField(Guid id, FieldEditDTO fieldEditDto)
        {
            try
            {
                var field = await _fieldService.UpdateField(id, fieldEditDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteField(Guid id)
        {
            try
            {
                await _fieldService.DeleteField(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
