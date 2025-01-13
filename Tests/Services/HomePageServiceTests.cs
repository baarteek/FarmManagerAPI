using System;
using System.Threading.Tasks;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Services
{
    public class HomePageServiceTests
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<IFertilizationRepository> _fertilizationRepositoryMock;
        private readonly Mock<IPlantProtectionRepository> _plantProtectionRepositoryMock;
        private readonly Mock<ICultivationOperationRepository> _cultivationOperationRepositoryMock;
        private readonly HomePageService _homePageService;

        public HomePageServiceTests()
        {
            _userManagerMock = TestHelper.CreateUserManagerMock<IdentityUser>();
            _fertilizationRepositoryMock = new Mock<IFertilizationRepository>();
            _plantProtectionRepositoryMock = new Mock<IPlantProtectionRepository>();
            _cultivationOperationRepositoryMock = new Mock<ICultivationOperationRepository>();

            _homePageService = new HomePageService(
                _userManagerMock.Object,
                _fertilizationRepositoryMock.Object,
                _plantProtectionRepositoryMock.Object,
                _cultivationOperationRepositoryMock.Object
            );
        }

        [Fact]
        public async Task GetHomePageInfo_ReturnsHomePageDTO_WhenDataExists()
        {
            var userName = "TestUser";
            var userId = Guid.NewGuid().ToString();
            var user = new IdentityUser { Id = userId, UserName = userName };

            var cultivationOperation = new CultivationOperation
            {
                Name = "Plowing",
                Description = "Deep plowing",
                Date = DateTime.Now,
                Crop = new Crop { Id = Guid.NewGuid(), Name = "Wheat" }
            };

            var fertilization = new Fertilization
            {
                Type = FertilizationType.NotSelected,
                Description = "Nitrogen fertilization",
                Date = DateTime.Now,
                Crop = new Crop { Id = Guid.NewGuid(), Name = "Barley" }
            };

            var plantProtection = new PlantProtection
            {
                Type = PlantProtectionType.Herbicide,
                Description = "Weed control",
                Date = DateTime.Now,
                Crop = new Crop { Id = Guid.NewGuid(), Name = "Corn" }
            };

            TestHelper.SetupUserManagerFindByName(_userManagerMock, userName, user);
            _cultivationOperationRepositoryMock.Setup(repo => repo.GetLatestCultivationOperationByUser(userId)).ReturnsAsync(cultivationOperation);
            _fertilizationRepositoryMock.Setup(repo => repo.GetLatestFertilizationByUser(userId)).ReturnsAsync(fertilization);
            _plantProtectionRepositoryMock.Setup(repo => repo.GetLatestPlantProtectionByUser(userId)).ReturnsAsync(plantProtection);

            var result = await _homePageService.GetHomePageInfo(userName);

            Assert.Equal(cultivationOperation.Name, result.CultivationOperationName);
            Assert.Equal(fertilization.Type.ToString(), result.FertilizationName);
            Assert.Equal(plantProtection.Type.ToString(), result.PlantProtectionName);
        }

        [Fact]
        public async Task GetHomePageInfo_ThrowsException_WhenUserNotFound()
        {
            var userName = "NonExistentUser";

            TestHelper.SetupUserManagerFindByName(_userManagerMock, userName, null);

            var exception = await Assert.ThrowsAsync<Exception>(() => _homePageService.GetHomePageInfo(userName));

            Assert.Equal($"User not found with username: {userName}", exception.Message);
        }

        [Fact]
        public async Task GetHomePageInfo_ReturnsDefaultValues_WhenNoDataExists()
        {
            var userName = "TestUser";
            var userId = Guid.NewGuid().ToString();
            var user = new IdentityUser { Id = userId, UserName = userName };

            TestHelper.SetupUserManagerFindByName(_userManagerMock, userName, user);
            _cultivationOperationRepositoryMock.Setup(repo => repo.GetLatestCultivationOperationByUser(userId)).ReturnsAsync((CultivationOperation?)null);
            _fertilizationRepositoryMock.Setup(repo => repo.GetLatestFertilizationByUser(userId)).ReturnsAsync((Fertilization?)null);
            _plantProtectionRepositoryMock.Setup(repo => repo.GetLatestPlantProtectionByUser(userId)).ReturnsAsync((PlantProtection?)null);

            var result = await _homePageService.GetHomePageInfo(userName);

            Assert.Equal("No Value", result.CultivationOperationName);
            Assert.Equal("No Value", result.FertilizationName);
            Assert.Equal("No Value", result.PlantProtectionName);
        }
    }
}