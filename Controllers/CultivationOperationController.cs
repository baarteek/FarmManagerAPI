using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagerAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CultivationOperationController : ControllerBase
{
    private readonly ICultivationOperationService _cultivationOperationService;

    public CultivationOperationController(ICultivationOperationService cultivationOperationService)
    {
        _cultivationOperationService = cultivationOperationService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CultivationOperationDTO>> GetCultivationOperation(Guid id)
    {
        try
        {
            var cultivationOperation = await _cultivationOperationService.GetCultivationOperationById(id);
            return Ok(cultivationOperation);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("crop/{cropId}")]
    public async Task<ActionResult<IEnumerable<CultivationOperationDTO>>> GetCultivationOperationsByCropId(Guid cropId)
    {
        try
        {
            var cultivationOperations = await _cultivationOperationService.GetCultivationOperationsByCropId(cropId);
            return Ok(cultivationOperations);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddCultivationOperation(CultivationOperationEditDTO cultivationOperation)
    {
        try
        {
            await _cultivationOperationService.AddCultivationOperation(cultivationOperation);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCultivationOperation(Guid id,
        CultivationOperationEditDTO cultivationOperation)
    {
        try
        {
            await _cultivationOperationService.UpdateCultivationOperation(id, cultivationOperation);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCultivationOperation(Guid id)
    {
        try
        {
            await _cultivationOperationService.DeleteCultivationOperation(id);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}