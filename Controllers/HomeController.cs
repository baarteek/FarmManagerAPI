using System.Security.Claims;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagerAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly IHomePageService _homePageService;

    public HomeController(IHomePageService homePageService)
    {
        _homePageService = homePageService;
    }

    [HttpGet("user")]
    public async Task<ActionResult<HomePageDTO>> GetHomePageInfo()
    {
        var userName = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userName is null)
        {
            return Unauthorized("User ID not found in token.");
        } 
        
        try
        {
            var homePageInfo = await _homePageService.GetHomePageInfo(userName);
            return Ok(homePageInfo);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}