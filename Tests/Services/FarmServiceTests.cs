using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Services
{
    public class FarmServiceTests
    {
        private readonly Mock<IFarmRepository> _farmRepositoryMock;
        private readonly Mock<IFieldRepository> _fieldRepositoryMock;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly FarmService _farmService;

        public FarmServiceTests()
        {
            _farmRepositoryMock = new Mock<IFarmRepository>();
            _fieldRepositoryMock = new Mock<IFieldRepository>();
            _userManagerMock = TestHelper.CreateUserManagerMock<IdentityUser>();

            _farmService = new FarmService(
                _farmRepositoryMock.Object,
                _userManagerMock.Object,
                _fieldRepositoryMock.Object);
        }

        [Fact]
        public async Task AddFarm_ReturnsFarmDTO_WhenSuccessful()
        {
            var farmEditDto = new FarmEditDTO { Name = "Test Farm", Location = "Test Location", TotalArea = 100.5 };
            var userName = "TestUser";
            var user = new IdentityUser { Id = "1", UserName = userName };
            var farmId = Guid.NewGuid();

            TestHelper.SetupUserManagerFindByName(_userManagerMock, userName, user);

            _farmRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<Farm>()))
                .Callback<Farm>(farm => farm.Id = farmId);

            var result = await _farmService.AddFarm(farmEditDto, userName);

            Assert.NotNull(result);
            Assert.Equal(farmId, result.Id);
            Assert.Equal(farmEditDto.Name, result.Name);
            Assert.Equal(farmEditDto.Location, result.Location);
            Assert.Equal(farmEditDto.TotalArea, result.TotalArea);
            Assert.Equal(userName, result.User.Name);
        }

        [Fact]
        public async Task AddFarm_ThrowsException_WhenUserNotFound()
        {
            var farmEditDto = new FarmEditDTO { Name = "Test Farm", Location = "Test Location", TotalArea = 100.5 };
            var userName = "InvalidUser";

            TestHelper.SetupUserManagerFindByName(_userManagerMock, userName, null);

            var exception = await Assert.ThrowsAsync<Exception>(() => _farmService.AddFarm(farmEditDto, userName));
            Assert.Equal($"User not found with username: {userName}", exception.Message);
        }

        [Fact]
        public async Task GetFarmById_ReturnsFarmDTO_WhenFarmExists()
        {
            var farmId = Guid.NewGuid();
            var user = new IdentityUser { Id = "1", UserName = "TestUser" };
            var farm = new Farm { Id = farmId, Name = "Test Farm", Location = "Test Location", TotalArea = 100.5, User = user };

            _farmRepositoryMock
                .Setup(repo => repo.GetById(farmId))
                .ReturnsAsync(farm);

            TestHelper.SetupUserManagerFindById(_userManagerMock, user.Id, user);

            _fieldRepositoryMock
                .Setup(repo => repo.GetFieldsByFarmId(farmId))
                .ReturnsAsync(new List<Field>());

            var result = await _farmService.GetFarmById(farmId);

            Assert.NotNull(result);
            Assert.Equal(farmId, result.Id);
            Assert.Equal(farm.Name, result.Name);
            Assert.Equal(farm.Location, result.Location);
            Assert.Equal(farm.TotalArea, result.TotalArea);
            Assert.Equal(user.UserName, result.User.Name);
        }

        [Fact]
        public async Task GetFarmById_ThrowsException_WhenFarmNotFound()
        {
            var farmId = Guid.NewGuid();

            _farmRepositoryMock
                .Setup(repo => repo.GetById(farmId))
                .ReturnsAsync((Farm)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _farmService.GetFarmById(farmId));
            Assert.Equal($"Farm not found with ID: {farmId}", exception.Message);
        }

        [Fact]
        public async Task GetFarmsByUser_ReturnsFarms_WhenUserExists()
        {
            var userName = "TestUser";
            var user = new IdentityUser { Id = "1", UserName = userName };
            var farms = new List<Farm>
            {
                new Farm { Id = Guid.NewGuid(), Name = "Farm1", Location = "Location1", TotalArea = 50.5, User = user },
                new Farm { Id = Guid.NewGuid(), Name = "Farm2", Location = "Location2", TotalArea = 100.0, User = user }
            };

            TestHelper.SetupUserManagerFindByName(_userManagerMock, userName, user);

            _farmRepositoryMock
                .Setup(repo => repo.GetFarmsByUser(user.Id))
                .ReturnsAsync(farms);

            foreach (var farm in farms)
            {
                _fieldRepositoryMock
                    .Setup(repo => repo.GetFieldsByFarmId(farm.Id))
                    .ReturnsAsync(new List<Field>());
            }

            var result = await _farmService.GetFarmsByUser(userName);

            Assert.NotNull(result);
            var farmDtos = result as FarmDTO[] ?? result.ToArray();
            Assert.Equal(farms.Count, farmDtos.Count());
            Assert.All(farmDtos, r => Assert.Equal(userName, r.User.Name));
        }

        [Fact]
        public async Task UpdateFarm_UpdatesFarm_WhenFarmExists()
        {
            var farmId = Guid.NewGuid();
            var farmEditDto = new FarmEditDTO { Name = "Updated Farm", Location = "Updated Location", TotalArea = 150.0 };
            var farm = new Farm { Id = farmId, Name = "Old Farm", Location = "Old Location", TotalArea = 100.0 };

            _farmRepositoryMock
                .Setup(repo => repo.GetById(farmId))
                .ReturnsAsync(farm);

            _farmRepositoryMock
                .Setup(repo => repo.Update(farm))
                .Returns(Task.CompletedTask);

            await _farmService.UpdateFarm(farmId, farmEditDto);

            _farmRepositoryMock.Verify(repo => repo.Update(It.Is<Farm>(f =>
                f.Name == farmEditDto.Name &&
                f.Location == farmEditDto.Location &&
                f.TotalArea == farmEditDto.TotalArea)), Times.Once);
        }

        [Fact]
        public async Task DeleteFarm_DeletesFarm()
        {
            var farmId = Guid.NewGuid();

            _farmRepositoryMock
                .Setup(repo => repo.Delete(farmId))
                .Returns(Task.CompletedTask);

            await _farmService.DeleteFarm(farmId);

            _farmRepositoryMock.Verify(repo => repo.Delete(farmId), Times.Once);
        }
    }
}