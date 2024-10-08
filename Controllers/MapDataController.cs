using System.Security.Claims;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FarmManagerAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class MapDataController : ControllerBase
{
    private readonly IMapDataService _mapDataService;

    public MapDataController(IMapDataService mapDataService)
    {
        _mapDataService = mapDataService;
    }

    [HttpGet("{farmId}")]
    public async Task<ActionResult<IEnumerable<MapDataDTO>>> GetMapData(Guid farmId) 
    {
        try
        {
            var mapData = await _mapDataService.GetMapDataByFarmId(farmId);
            if (mapData.IsNullOrEmpty())
            {
                return NoContent();
            }
            return Ok(mapData);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<MapDataDTO>>> GetUserMapData()
    {
        try
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null)
            {
                return Unauthorized("User ID not found in token.");
            }
            var mapData = await _mapDataService.GetMapDataByUser(userName);
            if (mapData.IsNullOrEmpty())
            {
                return NoContent();
            }
            return Ok(mapData);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}