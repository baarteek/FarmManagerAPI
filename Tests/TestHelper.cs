using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
    
    public static Mock<UserManager<TUser>> CreateUserManagerMock<TUser>() where TUser : class
    {
        var userStoreMock = new Mock<IUserStore<TUser>>();
        var userManagerMock = new Mock<UserManager<TUser>>(
            userStoreMock.Object,
            null!,
            null!, 
            null!, 
            null!,
            null!, 
            null!, 
            null!, 
            null!  
        );

        return userManagerMock;
    }

    public static void SetupUserManagerFindByName<TUser>(Mock<UserManager<TUser>> userManagerMock, string userName, TUser user) where TUser : class
    {
        userManagerMock
            .Setup(manager => manager.FindByNameAsync(userName))
            .ReturnsAsync(user);
    }

    public static void SetupUserManagerFindById<TUser>(Mock<UserManager<TUser>> userManagerMock, string userId, TUser user) where TUser : class
    {
        userManagerMock
            .Setup(manager => manager.FindByIdAsync(userId))
            .ReturnsAsync(user);
    }
}