using FarmManagerAPI.Controllers;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IHomePageService> _homePageServiceMock;
        private readonly HomeController _homeController;

        public HomeControllerTests()
        {
            _homePageServiceMock = new Mock<IHomePageService>();
            _homeController = new HomeController(_homePageServiceMock.Object);
        }

        [Fact]
        public async Task GetHomePageInfo_ReturnsOk_WhenUserExists()
        {
            var userId = "TestUser";
            TestHelper.SetUser(_homeController, userId);

            var homePageDto = new HomePageDTO
            {
                CultivationOperationName = "Plowing",
                CultivationOperationDescription = "Deep plowing of the field",
                CultivationOperationDate = DateTime.Now,
                CultivationOperationCrop = new MiniItemDTO { Id = Guid.NewGuid().ToString(), Name = "Wheat" },
                PlantProtectionName = "Herbicide",
                PlantProtectionDescription = "Weed control",
                PlantProtectionDate = DateTime.Now,
                PlantProtectionCrop = new MiniItemDTO { Id = Guid.NewGuid().ToString(), Name = "Corn" },
                FertilizationName = "Nitrogen",
                FertilizationDescription = "Nitrogen fertilization",
                FertilizationDate = DateTime.Now,
                FertilizationCrop = new MiniItemDTO { Id = Guid.NewGuid().ToString(), Name = "Barley" }
            };

            _homePageServiceMock
                .Setup(service => service.GetHomePageInfo(userId))
                .ReturnsAsync(homePageDto);

            var result = await _homeController.GetHomePageInfo();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedData = Assert.IsType<HomePageDTO>(okResult.Value);
            Assert.Equal(homePageDto.CultivationOperationName, returnedData.CultivationOperationName);
            Assert.Equal(homePageDto.PlantProtectionName, returnedData.PlantProtectionName);
            Assert.Equal(homePageDto.FertilizationName, returnedData.FertilizationName);
        }

        [Fact]
        public async Task GetHomePageInfo_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
        {
            TestHelper.SetNoUser(_homeController);

            var result = await _homeController.GetHomePageInfo();

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("User ID not found in token.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task GetHomePageInfo_ReturnsBadRequest_WhenServiceThrowsException()
        {
            var userId = "TestUser";
            TestHelper.SetUser(_homeController, userId);

            _homePageServiceMock
                .Setup(service => service.GetHomePageInfo(userId))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _homeController.GetHomePageInfo();

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Database error", badRequestResult.Value);
        }
    }
}