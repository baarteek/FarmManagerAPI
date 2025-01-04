using FarmManagerAPI.Controllers;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Controllers;

public class FarmsControllerTests
{
    private readonly Mock<IFarmService> _mockFarmService;
    private readonly FarmsController _farmsController;

    public FarmsControllerTests()
    {
        _mockFarmService = new Mock<IFarmService>();
        _farmsController = new FarmsController(_mockFarmService.Object);
    }

    [Fact]
    public async Task GetFarmsByUser_ReturnsOkWithFarms_WhenUserIsAuthenticated()
    {
        var userName = "testUser";
        var expectedFarms = new List<FarmDTO>
        {
            new FarmDTO { Id = Guid.NewGuid(), Name = "Farm 1" },
            new FarmDTO { Id = Guid.NewGuid(), Name = "Farm 2" },
        };
        
        _mockFarmService
            .Setup(service => service.GetFarmsByUser(userName))
            .ReturnsAsync(expectedFarms);
        
        TestHelper.SetUser(_farmsController, userName);

        var result = await _farmsController.GetFarmsByUser();
        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualFarms = Assert.IsType<List<FarmDTO>>(okResult.Value);
        Assert.Equal(expectedFarms.Count, actualFarms.Count);
        Assert.Equal(expectedFarms[0].Name, actualFarms[0].Name);
        Assert.Equal(expectedFarms[1].Name, actualFarms[1].Name);
    }

    [Fact]
    public async Task GetFarmsByUser_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        TestHelper.SetNoUser(_farmsController);
        
        var result = await _farmsController.GetFarmsByUser();
        
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.Equal("User ID not found in token.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task GetFarmById_ReturnsOkWithFarm()
    {
        var farmId = Guid.NewGuid();
        var farmDto = new FarmDTO { Id = farmId, Name = "Farm 1"};
        
        _mockFarmService
            .Setup(service => service.GetFarmById(farmId))
            .ReturnsAsync(farmDto);
        
        var result = await _farmsController.GetFarm(farmId);
        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualFarm = Assert.IsType<FarmDTO>(okResult.Value);
        Assert.Equal(farmDto.Id, actualFarm.Id);
        Assert.Equal(farmDto.Name, actualFarm.Name);
    }

    [Fact]
    public async Task GetFarmsNamesAndIdByUser_ReturnsOkWithFarmNames_WhenUserIsAuthenticated()
    {
        var userName = "testUser";
        var expectedList = new List<MiniItemDTO>
        {
            new MiniItemDTO { Id = Guid.NewGuid().ToString(), Name = "Farm 1" },
            new MiniItemDTO { Id = Guid.NewGuid().ToString(), Name = "Farm 2" },
        };
        
        _mockFarmService
            .Setup(service => service.GetFarmsNamesAndIdByUser(userName))
            .ReturnsAsync(expectedList);
        
        TestHelper.SetUser(_farmsController, userName);
        
        var result = await _farmsController.GetFarmsNamesAndIdByUser();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualList = Assert.IsType<List<MiniItemDTO>>(okResult.Value);
        Assert.Equal(expectedList.Count, actualList.Count);
        Assert.Equal(expectedList[0].Name, actualList[0].Name);
        Assert.Equal(expectedList[1].Name, actualList[1].Name);
    }

    [Fact]
    public async Task GetFarmsNamesAndIdByUser_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        TestHelper.SetNoUser(_farmsController);
        
        var result = await _farmsController.GetFarmsNamesAndIdByUser();
        
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.Equal("User ID not found in token.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task AddFarm_ReturnsOkWithFarm_WhenUserIsAuthenticated()
    {
        var userName = "test_user";
        var farmEditDto = new FarmEditDTO { Name = "New Farm", Location = "Test Location", TotalArea = 100 };
        var addedFarm = new FarmDTO
        {
            Id = Guid.NewGuid(),
            User = new MiniItemDTO { Id = "123", Name = userName },
            Name = farmEditDto.Name,
            Location = farmEditDto.Location,
            TotalArea = farmEditDto.TotalArea,
            Fields = new List<MiniItemDTO>()
        };

        _mockFarmService
            .Setup(service => service.AddFarm(farmEditDto, userName))
            .ReturnsAsync(addedFarm);

        TestHelper.SetUser(_farmsController, userName);
        
        var result = await _farmsController.AddFarm(farmEditDto);
        
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(FarmsController.GetFarm), createdAtActionResult.ActionName);

        var returnedFarm = Assert.IsType<FarmDTO>(createdAtActionResult.Value);
        Assert.Equal(addedFarm.Id, returnedFarm.Id);
        Assert.Equal(addedFarm.Name, returnedFarm.Name);
        Assert.Equal(addedFarm.Location, returnedFarm.Location);
        Assert.Equal(addedFarm.TotalArea, returnedFarm.TotalArea);
    }
    
    [Fact]
    public async Task AddFarm_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        var farmEditDto = new FarmEditDTO { Name = "New Farm", Location = "Test Location", TotalArea = 100 };

        TestHelper.SetNoUser(_farmsController);
        
        var result = await _farmsController.AddFarm(farmEditDto);
        
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("User ID not found in token.", unauthorizedResult.Value);
    }
    
    [Fact]
    public async Task AddFarm_ReturnsBadRequest_WhenServiceThrowsException()
    {
        var userName = "test_user";
        var farmEditDto = new FarmEditDTO { Name = "New Farm", Location = "Test Location", TotalArea = 100 };

        _mockFarmService
            .Setup(service => service.AddFarm(farmEditDto, userName))
            .ThrowsAsync(new Exception("Test exception"));

        TestHelper.SetUser(_farmsController, userName);
        
        var result = await _farmsController.AddFarm(farmEditDto);
        
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Test exception", badRequestResult.Value);
    }
    
    [Fact]
    public async Task UpdateFarm_ReturnsNoContent_WhenFarmIsUpdatedSuccessfully()
    {
        var farmId = Guid.NewGuid();
        var farmEditDto = new FarmEditDTO { Name = "Updated Farm", Location = "New Location", TotalArea = 150 };
        var userName = "test_user";

        var existingFarm = new FarmDTO
        {
            Id = farmId,
            Name = "Original Farm",
            User = new MiniItemDTO { Name = userName },
            Location = "Old Location",
            TotalArea = 100
        };

        _mockFarmService
            .Setup(service => service.GetFarmById(farmId))
            .ReturnsAsync(existingFarm);

        TestHelper.SetUser(_farmsController, userName);
        
        var result = await _farmsController.UpdateFarm(farmId, farmEditDto);
        
        Assert.IsType<NoContentResult>(result);

        _mockFarmService.Verify(service => service.GetFarmById(farmId), Times.Once);
        _mockFarmService.Verify(service => service.UpdateFarm(farmId, farmEditDto), Times.Once);
    }
    
    [Fact]
    public async Task UpdateFarm_ReturnsNotFound_WhenFarmDoesNotExist()
    {
        var farmId = Guid.NewGuid();
        var farmEditDto = new FarmEditDTO { Name = "Updated Farm", Location = "New Location", TotalArea = 150 };

        _mockFarmService
            .Setup(service => service.GetFarmById(farmId))
            .ReturnsAsync((FarmDTO)null!);

        TestHelper.SetUser(_farmsController, "test_user");
        
        var result = await _farmsController.UpdateFarm(farmId, farmEditDto);
        
        Assert.IsType<NotFoundResult>(result);

        _mockFarmService.Verify(service => service.GetFarmById(farmId), Times.Once);
        _mockFarmService.Verify(service => service.UpdateFarm(It.IsAny<Guid>(), It.IsAny<FarmEditDTO>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateFarm_ReturnsForbidden_WhenUserIsNotAuthenticated()
    {
        var farmId = Guid.NewGuid();
        var farmEditDto = new FarmEditDTO { Name = "Updated Farm", Location = "New Location", TotalArea = 150 };

        var existingFarm = new FarmDTO
        {
            Id = farmId,
            Name = "Original Farm",
            User = new MiniItemDTO { Name = "authorized_user" },
            Location = "Old Location",
            TotalArea = 100
        };

        _mockFarmService
            .Setup(service => service.GetFarmById(farmId))
            .ReturnsAsync(existingFarm);

        TestHelper.SetNoUser(_farmsController);
        
        var result = await _farmsController.UpdateFarm(farmId, farmEditDto);

        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task UpdateFarm_ReturnsForbidden_WhenUserIsNotFarmOwner()
    {
        var farmId = Guid.NewGuid();
        var farmEditDto = new FarmEditDTO { Name = "Updated Farm", Location = "New Location", TotalArea = 150 };
        var userName = "unauthorized_user";

        var existingFarm = new FarmDTO
        {
            Id = farmId,
            Name = "Original Farm",
            User = new MiniItemDTO { Name = "authorized_user" },
            Location = "Old Location",
            TotalArea = 100
        };

        _mockFarmService
            .Setup(service => service.GetFarmById(farmId))
            .ReturnsAsync(existingFarm);

        TestHelper.SetUser(_farmsController, userName);

        var result = await _farmsController.UpdateFarm(farmId, farmEditDto);

       Assert.IsType<ForbidResult>(result);
    }
    
    [Fact]
    public async Task DeleteFarm_ReturnsNoContent_WhenFarmIsDeletedSuccessfully()
    {
        var farmId = Guid.NewGuid();
        var userName = "test_user";

        var existingFarm = new FarmDTO
        {
            Id = farmId,
            Name = "Farm to delete",
            User = new MiniItemDTO { Name = userName }
        };

        _mockFarmService
            .Setup(service => service.GetFarmById(farmId))
            .ReturnsAsync(existingFarm);

        TestHelper.SetUser(_farmsController, userName);
        
        var result = await _farmsController.DeleteFarm(farmId);

        Assert.IsType<NoContentResult>(result);
    }
    
    
    [Fact]
    public async Task DeleteFarm_ReturnsNotFound_WhenFarmDoesNotExist()
    {
        var farmId = Guid.NewGuid();

        _mockFarmService
            .Setup(service => service.GetFarmById(farmId))
            .ReturnsAsync((FarmDTO)null!);

        TestHelper.SetUser(_farmsController, "test_user");
        
        var result = await _farmsController.DeleteFarm(farmId);
        
        Assert.IsType<NotFoundResult>(result);

        _mockFarmService.Verify(service => service.GetFarmById(farmId), Times.Once);
        _mockFarmService.Verify(service => service.DeleteFarm(It.IsAny<Guid>()), Times.Never);
    }
    
    [Fact]
    public async Task DeleteFarm_ReturnsForbidden_WhenUserIsNotAuthenticated()
    {
        var farmId = Guid.NewGuid();

        var existingFarm = new FarmDTO
        {
            Id = farmId,
            Name = "Farm to delete",
            User = new MiniItemDTO { Name = "authorized_user" }
        };

        _mockFarmService
            .Setup(service => service.GetFarmById(farmId))
            .ReturnsAsync(existingFarm);

        TestHelper.SetNoUser(_farmsController);
        
        var result = await _farmsController.DeleteFarm(farmId);
        
        Assert.IsType<ForbidResult>(result);
    }
    
    [Fact]
    public async Task DeleteFarm_ReturnsForbidden_WhenUserIsNotFarmOwner()
    {
        var farmId = Guid.NewGuid();
        var userName = "unauthorized_user";

        var existingFarm = new FarmDTO
        {
            Id = farmId,
            Name = "Farm to delete",
            User = new MiniItemDTO { Name = "authorized_user" }
        };

        _mockFarmService
            .Setup(service => service.GetFarmById(farmId))
            .ReturnsAsync(existingFarm);

        TestHelper.SetUser(_farmsController, userName);
        
        var result = await _farmsController.DeleteFarm(farmId);
        
        Assert.IsType<ForbidResult>(result);
    }
}