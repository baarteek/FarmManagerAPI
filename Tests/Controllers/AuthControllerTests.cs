using FarmManagerAPI.Controllers;
using FarmManagerAPI.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace FarmManagerAPI.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
    private readonly Mock<SignInManager<IdentityUser>> _siginInManagerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            Mock.Of<IUserStore<IdentityUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);

        _siginInManagerMock = new Mock<SignInManager<IdentityUser>>(
            _userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(),
            null!, null!, null!, null!);
        
        _configurationMock = new Mock<IConfiguration>();
        
        _authController = new AuthController(
            _userManagerMock.Object,
            _siginInManagerMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task Register_ValidUser_ReturnsOk()
    {
        var registerDto = new RegisterDTO
        {
            Username = "testUser",
            Email = "test@test.com",
            Password = "p@ssw0rd",
        };
        
        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Success);
        
        var result = await _authController.Register(registerDto);
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Register_InvalidModeState_ReturnsBadRequest()
    {
        _authController.ModelState.AddModelError("Username", "Required");
        var registerDto = new RegisterDTO();
        var result = await _authController.Register(registerDto);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task Register_CreateUserFails_ReturnsBadRequestWithErrors()
    {
        var registerDto = new RegisterDTO
        {
            Username = "testUser",
            Email = "test@example.com",
            Password = "P@ssw0rd"
        };

        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));
        
        var result = await _authController.Register(registerDto);
        
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var loginDto = new LoginDTO
        {
            Email = "test@example.com",
            Password = "P@ssw0rd"
        };

        var user = new IdentityUser { UserName = "testUser", Email = "test@example.com" };
        
        _userManagerMock
            .Setup(um => um.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _siginInManagerMock
            .Setup(sm => sm.CheckPasswordSignInAsync(user, loginDto.Password, false))
            .ReturnsAsync(SignInResult.Success);
        
        _configurationMock
            .Setup(c => c["Jwt:Key"])
            .Returns("eyJhbGciOiJIUzI1NiJ9.eyJSb2xlIjoiQWRtaW4iLCJJc3N1ZXIiOiJGYXJtTWFuYWdlciIsImlhdCI6MTcyMjY3OTU1MH0.u3Ts1ANA0LgKbO1ldN7wbxLT9Ye9MYSJH63mfIjtXTg");
        _configurationMock
            .Setup(c => c["Jwt:Issuer"])
            .Returns("testissuer");
        
        var result = await _authController.Login(loginDto);
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        var token = ((dynamic)okResult.Value!).Token;
        Assert.NotNull(token);
    }
    
    [Fact]
    public async Task Login_UserNotFound_ReturnsUnauthorized()
    {
        var loginDto = new LoginDTO
        {
            Email = "test@example.com",
            Password = "P@ssw0rd"
        };

        _userManagerMock
            .Setup(um => um.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync((IdentityUser)null!);
        
        var result = await _authController.Login(loginDto);
        
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        var loginDto = new LoginDTO
        {
            Email = "test@example.com",
            Password = "P@ssw0rd"
        };

        var user = new IdentityUser { UserName = "testUser", Email = "test@example.com" };

        _userManagerMock
            .Setup(um => um.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _siginInManagerMock
            .Setup(sm => sm.CheckPasswordSignInAsync(user, loginDto.Password, false))
            .ReturnsAsync(SignInResult.Failed);
        
        var result = await _authController.Login(loginDto);
        
        Assert.IsType<UnauthorizedResult>(result);
    }
}