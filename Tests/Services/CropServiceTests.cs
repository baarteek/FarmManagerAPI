using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Services
{
    public class CropServiceTests
    {
        private readonly Mock<ICropRepository> _cropRepositoryMock;
        private readonly Mock<IFieldRepository> _fieldRepositoryMock;
        private readonly Mock<IFertilizationRepository> _fertilizationRepositoryMock;
        private readonly Mock<IPlantProtectionRepository> _plantProtectionRepositoryMock;
        private readonly Mock<ICultivationOperationRepository> _cultivationOperationRepositoryMock;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly CropService _cropService;

        public CropServiceTests()
        {
            _cropRepositoryMock = new Mock<ICropRepository>();
            _fieldRepositoryMock = new Mock<IFieldRepository>();
            _fertilizationRepositoryMock = new Mock<IFertilizationRepository>();
            _plantProtectionRepositoryMock = new Mock<IPlantProtectionRepository>();
            _cultivationOperationRepositoryMock = new Mock<ICultivationOperationRepository>();
            _userManagerMock = TestHelper.CreateUserManagerMock<IdentityUser>();

            _cropService = new CropService(
                _cropRepositoryMock.Object,
                _fieldRepositoryMock.Object,
                _fertilizationRepositoryMock.Object,
                _plantProtectionRepositoryMock.Object,
                _cultivationOperationRepositoryMock.Object,
                _userManagerMock.Object
            );
        }

        [Fact]
        public async Task AddCrop_ReturnsCropDTO_WhenSuccessful()
        {
            var cropEditDto = new CropEditDTO
            {
                Name = "Test Crop",
                FieldId = Guid.NewGuid(),
                IsActive = true
            };

            var field = new Field { Id = cropEditDto.FieldId, Name = "Test Field" };

            _fieldRepositoryMock
                .Setup(repo => repo.GetById(cropEditDto.FieldId))
                .ReturnsAsync(field);

            _cropRepositoryMock
                .Setup(repo => repo.GetCropsByFieldId(cropEditDto.FieldId))
                .ReturnsAsync(new List<Crop>());

            _cropRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<Crop>()))
                .Callback<Crop>(crop => crop.Id = Guid.NewGuid());

            var result = await _cropService.AddCrop(cropEditDto);

            Assert.NotNull(result);
            Assert.Equal(cropEditDto.Name, result.Name);
            Assert.Equal(field.Id.ToString(), result.Field.Id);
        }

        [Fact]
        public async Task AddCrop_ThrowsException_WhenFieldNotFound()
        {
            var cropEditDto = new CropEditDTO { FieldId = Guid.NewGuid() };

            _fieldRepositoryMock
                .Setup(repo => repo.GetById(cropEditDto.FieldId))
                .ReturnsAsync((Field)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _cropService.AddCrop(cropEditDto));
            Assert.Equal($"Field not found with ID: {cropEditDto.FieldId}", exception.Message);
        }

        [Fact]
        public async Task GetCropById_ReturnsCropDTO_WhenCropExists()
        {
            var cropId = Guid.NewGuid();
            var field = new Field { Id = Guid.NewGuid(), Name = "Test Field" };
            var crop = new Crop
            {
                Id = cropId,
                Name = "Test Crop",
                IsActive = true,
                Field = field
            };

            _cropRepositoryMock
                .Setup(repo => repo.GetById(cropId))
                .ReturnsAsync(crop);

            _fertilizationRepositoryMock
                .Setup(repo => repo.GetFertilizationsByCropId(cropId))
                .ReturnsAsync(new List<Fertilization>());

            _plantProtectionRepositoryMock
                .Setup(repo => repo.GetPlantProtectionsByCropId(cropId))
                .ReturnsAsync(new List<PlantProtection>());

            _cultivationOperationRepositoryMock
                .Setup(repo => repo.GetCultivationOperationsByCropId(cropId))
                .ReturnsAsync(new List<CultivationOperation>());

            var result = await _cropService.GetCropById(cropId);

            Assert.NotNull(result);
            Assert.Equal(crop.Name, result.Name);
            Assert.Equal(field.Id.ToString(), result.Field.Id);
        }

        [Fact]
        public async Task GetCropById_ThrowsException_WhenCropNotFound()
        {
            var cropId = Guid.NewGuid();

            _cropRepositoryMock
                .Setup(repo => repo.GetById(cropId))
                .ReturnsAsync((Crop)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _cropService.GetCropById(cropId));
            Assert.Equal($"Crop not found with ID: {cropId}", exception.Message);
        }

        [Fact]
        public async Task UpdateCrop_UpdatesCrop_WhenSuccessful()
        {
            var cropId = Guid.NewGuid();
            var cropEditDto = new CropEditDTO
            {
                Name = "Updated Crop",
                IsActive = true,
                FieldId = Guid.NewGuid()
            };

            var crop = new Crop { Id = cropId, Name = "Old Crop", IsActive = false };

            _cropRepositoryMock
                .Setup(repo => repo.GetById(cropId))
                .ReturnsAsync(crop);

            _cropRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<Crop>()))
                .Returns(Task.CompletedTask);

            await _cropService.UpdateCrop(cropId, cropEditDto);

            _cropRepositoryMock.Verify(repo => repo.Update(It.Is<Crop>(c =>
                c.Name == cropEditDto.Name &&
                c.IsActive == cropEditDto.IsActive)), Times.Once);
        }

        [Fact]
        public async Task DeleteCrop_DeletesCrop_WhenSuccessful()
        {
            var cropId = Guid.NewGuid();

            _cropRepositoryMock
                .Setup(repo => repo.Delete(cropId))
                .Returns(Task.CompletedTask);

            await _cropService.DeleteCrop(cropId);

            _cropRepositoryMock.Verify(repo => repo.Delete(cropId), Times.Once);
        }

        [Fact]
        public async Task GetActiveCropsByUser_ReturnsActiveCrops_WhenUserExists()
        {
            var userName = "TestUser";
            var user = new IdentityUser { Id = "1", UserName = userName };
            var crops = new List<Crop>
            {
                new Crop { Id = Guid.NewGuid(), Name = "Crop1", IsActive = true, Field = new Field { Id = Guid.NewGuid(), Name = "Field1" } },
                new Crop { Id = Guid.NewGuid(), Name = "Crop2", IsActive = true, Field = new Field { Id = Guid.NewGuid(), Name = "Field2" } }
            };

            TestHelper.SetupUserManagerFindByName(_userManagerMock, userName, user);

            _cropRepositoryMock
                .Setup(repo => repo.GetActiveCropsByUserId(user.Id))
                .ReturnsAsync(crops);

            var result = await _cropService.GetActiveCropsByUser(userName);

            Assert.NotNull(result);
            Assert.Equal(crops.Count, result.Count());
        }
    }
}