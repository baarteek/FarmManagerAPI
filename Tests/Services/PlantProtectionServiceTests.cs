using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Implementations;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Services
{
    public class PlantProtectionServiceTests
    {
        private readonly Mock<IPlantProtectionRepository> _plantProtectionRepositoryMock;
        private readonly Mock<ICropRepository> _cropRepositoryMock;
        private readonly PlantProtectionService _plantProtectionService;

        public PlantProtectionServiceTests()
        {
            _plantProtectionRepositoryMock = new Mock<IPlantProtectionRepository>();
            _cropRepositoryMock = new Mock<ICropRepository>();
            _plantProtectionService = new PlantProtectionService(
                _plantProtectionRepositoryMock.Object,
                _cropRepositoryMock.Object
            );
        }

        [Fact]
        public async Task AddPlantProtection_ReturnsDTO_WhenSuccessful()
        {
            var cropId = Guid.NewGuid();
            var crop = new Crop { Id = cropId, Name = "Test Crop" };
            var plantProtectionEditDto = new PlantProtectionEditDTO
            {
                CropId = cropId,
                Date = DateTime.Now,
                Type = PlantProtectionType.Herbicide,
                AgrotechnicalIntervention = AgrotechnicalIntervention.None,
                Quantity = 20.0,
                NameOfProduct = "Test Product",
                Description = "Test Description"
            };

            _cropRepositoryMock
                .Setup(repo => repo.GetById(cropId))
                .ReturnsAsync(crop);

            _plantProtectionRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<PlantProtection>()))
                .Returns(Task.CompletedTask);

            var result = await _plantProtectionService.AddPlantProtection(plantProtectionEditDto);

            Assert.NotNull(result);
            Assert.Equal(plantProtectionEditDto.NameOfProduct, result.NameOfProduct);
            Assert.Equal(crop.Id.ToString(), result.Crop.Id);
        }

        [Fact]
        public async Task AddPlantProtection_ThrowsException_WhenCropNotFound()
        {
            var plantProtectionEditDto = new PlantProtectionEditDTO { CropId = Guid.NewGuid() };

            _cropRepositoryMock
                .Setup(repo => repo.GetById(plantProtectionEditDto.CropId))
                .ReturnsAsync((Crop)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _plantProtectionService.AddPlantProtection(plantProtectionEditDto));

            Assert.Equal("Crop not found", exception.Message);
        }

        [Fact]
        public async Task GetPlantProtectionById_ReturnsDTO_WhenPlantProtectionExists()
        {
            var id = Guid.NewGuid();
            var crop = new Crop { Id = Guid.NewGuid(), Name = "Test Crop" };
            var plantProtection = new PlantProtection
            {
                Id = id,
                Crop = crop,
                Date = DateTime.Now,
                Type = PlantProtectionType.Herbicide,
                NameOfProduct = "Test Product",
                Quantity = 20.0,
                Description = "Test Description"
            };

            _plantProtectionRepositoryMock
                .Setup(repo => repo.GetById(id))
                .ReturnsAsync(plantProtection);

            var result = await _plantProtectionService.GetPlantProtectionById(id);

            Assert.NotNull(result);
            Assert.Equal(plantProtection.Id, result.Id);
            Assert.Equal(plantProtection.NameOfProduct, result.NameOfProduct);
        }

        [Fact]
        public async Task GetPlantProtectionById_ThrowsException_WhenPlantProtectionNotFound()
        {
            var id = Guid.NewGuid();

            _plantProtectionRepositoryMock
                .Setup(repo => repo.GetById(id))
                .ReturnsAsync((PlantProtection)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _plantProtectionService.GetPlantProtectionById(id));

            Assert.Equal("PlantProtection not found", exception.Message);
        }

        [Fact]
        public async Task GetPlantProtectionsByCropId_ReturnsListOfDTOs_WhenProtectionsExist()
        {
            var cropId = Guid.NewGuid();
            var crop = new Crop { Id = cropId, Name = "Test Crop" };
            var plantProtections = new List<PlantProtection>
            {
                new PlantProtection
                {
                    Id = Guid.NewGuid(),
                    Crop = crop,
                    Date = DateTime.Now,
                    Type = PlantProtectionType.Fungicide,
                    NameOfProduct = "Product 1",
                    Quantity = 15.0,
                    Description = "Description 1"
                },
                new PlantProtection
                {
                    Id = Guid.NewGuid(),
                    Crop = crop,
                    Date = DateTime.Now,
                    Type = PlantProtectionType.Insecticide,
                    NameOfProduct = "Product 2",
                    Quantity = 25.0,
                    Description = "Description 2"
                }
            };

            _plantProtectionRepositoryMock
                .Setup(repo => repo.GetPlantProtectionsByCropId(cropId))
                .ReturnsAsync(plantProtections);

            var result = await _plantProtectionService.GetPlantProtectionsByCropId(cropId);

            Assert.NotNull(result);
            IEnumerable<PlantProtectionDTO> plantProtectionDtos = result as PlantProtectionDTO[] ?? result.ToArray();
            Assert.Equal(plantProtections.Count, plantProtectionDtos.Count());
            Assert.Contains(plantProtectionDtos, pp => pp.NameOfProduct == "Product 1");
        }

        [Fact]
        public async Task DeletePlantProtection_DeletesSuccessfully_WhenProtectionExists()
        {
            var id = Guid.NewGuid();

            _plantProtectionRepositoryMock
                .Setup(repo => repo.Delete(id))
                .Returns(Task.CompletedTask);

            await _plantProtectionService.DeletePlantProtection(id);

            _plantProtectionRepositoryMock.Verify(repo => repo.Delete(id), Times.Once);
        }

        [Fact]
        public async Task UpdatePlantProtection_ReturnsDTO_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var cropId = Guid.NewGuid();
            var crop = new Crop { Id = cropId, Name = "Test Crop" };
            var plantProtection = new PlantProtection
            {
                Id = id,
                Crop = crop,
                Date = DateTime.Now,
                Type = PlantProtectionType.Herbicide,
                NameOfProduct = "Old Product",
                Quantity = 10.0,
                Description = "Old Description"
            };

            var plantProtectionEditDto = new PlantProtectionEditDTO
            {
                CropId = cropId,
                Date = DateTime.Now,
                Type = PlantProtectionType.Fungicide,
                NameOfProduct = "New Product",
                Quantity = 20.0,
                Description = "New Description"
            };

            _plantProtectionRepositoryMock
                .Setup(repo => repo.GetById(id))
                .ReturnsAsync(plantProtection);

            _cropRepositoryMock
                .Setup(repo => repo.GetById(cropId))
                .ReturnsAsync(crop);

            _plantProtectionRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<PlantProtection>()))
                .Returns(Task.CompletedTask);

            var result = await _plantProtectionService.UpdatePlantProtection(id, plantProtectionEditDto);

            Assert.NotNull(result);
            Assert.Equal(plantProtectionEditDto.NameOfProduct, result.NameOfProduct);
            Assert.Equal(plantProtectionEditDto.Quantity, result.Quantity);
        }
    }
}