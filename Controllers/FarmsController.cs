using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FarmManagerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class FarmsController : ControllerBase
    {
        private readonly IFarmService _farmService;

        public FarmsController(IFarmService farmService)
        {
            _farmService = farmService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FarmDTO>> GetFarm(Guid id)
        {
            var farm = await _farmService.GetFarmById(id);
            if (farm == null)
            {
                return NotFound();
            }
            return Ok(farm);
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<FarmDTO>>> GetFarmsByUser()
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null)
            {
                return Unauthorized("User ID not found in token.");
            }
            var farms = await _farmService.GetFarmsByUser(userName);
            return Ok(farms);
        }

        [HttpGet("user/list")]
        public async Task<ActionResult<IEnumerable<MiniItemDTO>>> GetFarmsNamesAndIdByUser()
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userName == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var farms = await _farmService.GetFarmsNamesAndIdByUser(userName);
            return Ok(farms);
        }

        [HttpPost]
        public async Task<ActionResult> AddFarm(FarmEditDTO farmEditDto)
        {
            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null)
            {
                return Unauthorized("User ID not found in token.");
            }
            try
            {
                var farm = await _farmService.AddFarm(farmEditDto, userName);
                return CreatedAtAction(nameof(GetFarm), new { id = farm.Id }, new FarmDTO
                {
                    Id = farm.Id,
                    User = new MiniItemDTO { Id = farm.User.Id, Name = farm.User.Name },
                    Name = farm.Name,
                    Location = farm.Location,
                    TotalArea = farm.TotalArea,
                    Fields = farm.Fields != null ? farm.Fields.Select(f => new MiniItemDTO { Id = f.Id.ToString(), Name = f.Name }).ToList() : new List<MiniItemDTO>()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateFarm(Guid id, FarmEditDTO farmEditDto)
        {
            var farm = await _farmService.GetFarmById(id);
            if (farm == null)
            {
                return NotFound();
            }

            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null || farm.User.Name != userName)
            {
                return Forbid("You are not authorized to update this farm.");
            }

            await _farmService.UpdateFarm(id, farmEditDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFarm(Guid id)
        {
            var farmDto = await _farmService.GetFarmById(id);
            if (farmDto == null)
            {
                return NotFound();
            }

            var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userName == null || farmDto.User.Name != userName)
            {
                return Forbid("You are not authorized to delete this farm.");
            }

            await _farmService.DeleteFarm(id);
            return NoContent();
        }
    }
}
