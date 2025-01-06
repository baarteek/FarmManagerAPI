using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Services
{
    public class MapDataServiceTests
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<IFieldRepository> _fieldRepositoryMock;
        private readonly Mock<ICropRepository> _cropRepositoryMock;
        private readonly MapDataService _mapDataService;

        public MapDataServiceTests()
        {
            _userManagerMock = TestHelper.CreateUserManagerMock<IdentityUser>();
            _fieldRepositoryMock = new Mock<IFieldRepository>();
            _cropRepositoryMock = new Mock<ICropRepository>();
            _mapDataService = new MapDataService(
                _userManagerMock.Object,
                _fieldRepositoryMock.Object,
                _cropRepositoryMock.Object
            );
        }

        [Fact]
        public async Task GetMapDataByFarmId_ReturnsMapData_WhenFieldsExist()
        {
            var farmId = Guid.NewGuid();
            var fields = new List<Field>
            {
                new Field { Id = Guid.NewGuid(), Name = "Field 1", Area = 100, Coordinates = "Coord1" },
                new Field { Id = Guid.NewGuid(), Name = "Field 2", Area = 200, Coordinates = "Coord2" }
            };
            var crops = new List<Crop>
            {
                new Crop { Id = Guid.NewGuid(), Name = "Crop 1" },
                new Crop { Id = Guid.NewGuid(), Name = "Crop 2" }
            };

            _fieldRepositoryMock
                .Setup(repo => repo.GetFieldsByFarmId(farmId))
                .ReturnsAsync(fields);

            _cropRepositoryMock
                .Setup(repo => repo.GetActiveCropByFieldId(fields[0].Id))
                .ReturnsAsync(crops[0]);

            _cropRepositoryMock
                .Setup(repo => repo.GetActiveCropByFieldId(fields[1].Id))
                .ReturnsAsync(crops[1]);

            var result = await _mapDataService.GetMapDataByFarmId(farmId);

            Assert.NotNull(result);
            IEnumerable<MapDataDTO> mapDataDtos = result as MapDataDTO[] ?? result.ToArray();
            Assert.Equal(fields.Count, mapDataDtos.Count());
            Assert.Contains(mapDataDtos, data => data.FieldName == "Field 1" && data.CropName == "Crop 1");
        }

        [Fact]
        public async Task GetMapDataByFarmId_ReturnsEmptyList_WhenNoFieldsExist()
        {
            var farmId = Guid.NewGuid();

            _fieldRepositoryMock
                .Setup(repo => repo.GetFieldsByFarmId(farmId))
                .ReturnsAsync(new List<Field>());

            var result = await _mapDataService.GetMapDataByFarmId(farmId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetMapDataByUser_ReturnsMapData_WhenFieldsExist()
        {
            var userName = "TestUser";
            var user = new IdentityUser { Id = "1", UserName = userName };
            var fields = new List<Field>
            {
                new Field { Id = Guid.NewGuid(), Name = "Field 1", Area = 100, Coordinates = "Coord1" },
                new Field { Id = Guid.NewGuid(), Name = "Field 2", Area = 200, Coordinates = "Coord2" }
            };
            var crops = new List<Crop>
            {
                new Crop { Id = Guid.NewGuid(), Name = "Crop 1" },
                new Crop { Id = Guid.NewGuid(), Name = "Crop 2" }
            };

            _userManagerMock
                .Setup(manager => manager.FindByNameAsync(userName))
                .ReturnsAsync(user);

            _fieldRepositoryMock
                .Setup(repo => repo.GetFieldsByUser(user.Id))
                .ReturnsAsync(fields);

            _cropRepositoryMock
                .Setup(repo => repo.GetActiveCropByFieldId(fields[0].Id))
                .ReturnsAsync(crops[0]);

            _cropRepositoryMock
                .Setup(repo => repo.GetActiveCropByFieldId(fields[1].Id))
                .ReturnsAsync(crops[1]);

            var result = await _mapDataService.GetMapDataByUser(userName);

            Assert.NotNull(result);
            IEnumerable<MapDataDTO> mapDataDtos = result as MapDataDTO[] ?? result.ToArray();
            Assert.Equal(fields.Count, mapDataDtos.Count());
            Assert.Contains(mapDataDtos, data => data.FieldName == "Field 1" && data.CropName == "Crop 1");
        }

        [Fact]
        public async Task GetMapDataByUser_ThrowsException_WhenUserNotFound()
        {
            var userName = "TestUser";

            _userManagerMock
                .Setup(manager => manager.FindByNameAsync(userName))
                .ReturnsAsync((IdentityUser)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _mapDataService.GetMapDataByUser(userName));

            Assert.Equal($"User not found with username: {userName}", exception.Message);
        }

        [Fact]
        public async Task GetMapDataByUser_ReturnsEmptyList_WhenNoFieldsExist()
        {
            var userName = "TestUser";
            var user = new IdentityUser { Id = "1", UserName = userName };

            _userManagerMock
                .Setup(manager => manager.FindByNameAsync(userName))
                .ReturnsAsync(user);

            _fieldRepositoryMock
                .Setup(repo => repo.GetFieldsByUser(user.Id))
                .ReturnsAsync(new List<Field>());

            var result = await _mapDataService.GetMapDataByUser(userName);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}