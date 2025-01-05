using System.Security.Claims;
using FarmManagerAPI.Controllers;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Controllers
{
    public class MapDataControllerTests
    {
        private readonly Mock<IMapDataService> _mapDataServiceMock;
        private readonly MapDataController _controller;

        public MapDataControllerTests()
        {
            _mapDataServiceMock = new Mock<IMapDataService>();
            _controller = new MapDataController(_mapDataServiceMock.Object);
        }

        private void SetUser(ControllerBase controller, string userName)
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

        private void SetNoUser(ControllerBase controller)
        {
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };
        }

        [Fact]
        public async Task GetMapData_ReturnsOk_WhenDataExists()
        {
            var farmId = Guid.NewGuid();
            var mapData = new List<MapDataDTO>
            {
                new MapDataDTO {  FieldName = "Map1", Area = 100.0 },
                new MapDataDTO {  FieldName = "Map2", Area = 200.0 }
            };

            _mapDataServiceMock
                .Setup(service => service.GetMapDataByFarmId(farmId))
                .ReturnsAsync(mapData);

            var result = await _controller.GetMapData(farmId);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedMapData = Assert.IsType<List<MapDataDTO>>(actionResult.Value);
            Assert.Equal(mapData.Count, returnedMapData.Count);
        }

        [Fact]
        public async Task GetMapData_ReturnsNoContent_WhenDataIsEmpty()
        {
            var farmId = Guid.NewGuid();

            _mapDataServiceMock
                .Setup(service => service.GetMapDataByFarmId(farmId))
                .ReturnsAsync(new List<MapDataDTO>());

            var result = await _controller.GetMapData(farmId);
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task GetMapData_ReturnsBadRequest_WhenExceptionThrown()
        {
            var farmId = Guid.NewGuid();

            _mapDataServiceMock
                .Setup(service => service.GetMapDataByFarmId(farmId))
                .ThrowsAsync(new Exception("Error fetching map data"));

            var result = await _controller.GetMapData(farmId);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error fetching map data", badRequestResult.Value);
        }

        [Fact]
        public async Task GetUserMapData_ReturnsOk_WhenDataExists()
        {
            SetUser(_controller, "TestUser");

            var mapData = new List<MapDataDTO>
            {
                new MapDataDTO {  FieldName = "Map1", Area = 100.0 },
                new MapDataDTO {  FieldName = "Map2", Area = 200.0 }
            };

            _mapDataServiceMock
                .Setup(service => service.GetMapDataByUser("TestUser"))
                .ReturnsAsync(mapData);

            var result = await _controller.GetUserMapData();
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedMapData = Assert.IsType<List<MapDataDTO>>(actionResult.Value);
            Assert.Equal(mapData.Count, returnedMapData.Count);
        }

        [Fact]
        public async Task GetUserMapData_ReturnsNoContent_WhenDataIsEmpty()
        {
            SetUser(_controller, "TestUser");

            _mapDataServiceMock
                .Setup(service => service.GetMapDataByUser("TestUser"))
                .ReturnsAsync(new List<MapDataDTO>());

            var result = await _controller.GetUserMapData();
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task GetUserMapData_ReturnsUnauthorized_WhenUserNotFound()
        {
            SetNoUser(_controller);

            var result = await _controller.GetUserMapData();
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("User ID not found in token.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task GetUserMapData_ReturnsBadRequest_WhenExceptionThrown()
        {
            SetUser(_controller, "TestUser");

            _mapDataServiceMock
                .Setup(service => service.GetMapDataByUser("TestUser"))
                .ThrowsAsync(new Exception("Error fetching user map data"));

            var result = await _controller.GetUserMapData();
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error fetching user map data", badRequestResult.Value);
        }
    }
}