using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagerAPI.Tests;

public static class TestHelper
{
    public static void SetUser(ControllerBase controller, string userName)
    {
        var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userName)
        }));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = userClaims }
        };
    }

    public static void SetNoUser(ControllerBase controller)
    {
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };
    }
}