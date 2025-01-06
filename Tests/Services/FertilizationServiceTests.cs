using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Implementations;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Services
{
    public class FertilizationServiceTests
    {
        private readonly Mock<IFertilizationRepository> _fertilizationRepositoryMock;
        private readonly Mock<ICropRepository> _cropRepositoryMock;
        private readonly FertilizationService _fertilizationService;

        public FertilizationServiceTests()
        {
            _fertilizationRepositoryMock = new Mock<IFertilizationRepository>();
            _cropRepositoryMock = new Mock<ICropRepository>();
            _fertilizationService = new FertilizationService(
                _fertilizationRepositoryMock.Object,
                _cropRepositoryMock.Object
            );
        }

        [Fact]
        public async Task AddFertilization_ReturnsDTO_WhenSuccessful()
        {
            var cropId = Guid.NewGuid();
            var crop = new Crop { Id = cropId, Name = "Test Crop" };
            var fertilizationEditDto = new FertilizationEditDTO
            {
                CropId = cropId,
                Date = DateTime.Now,
                Type = FertilizationType.Organic,
                AgrotechnicalIntervention = AgrotechnicalIntervention.None,
                NameOfProduct = "Test Product",
                Quantity = 10.5,
                Description = "Test Description"
            };

            _cropRepositoryMock
                .Setup(repo => repo.GetById(cropId))
                .ReturnsAsync(crop);

            _fertilizationRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<Fertilization>()))
                .Returns(Task.CompletedTask);

            var result = await _fertilizationService.AddFertilization(fertilizationEditDto);

            Assert.NotNull(result);
            Assert.Equal(fertilizationEditDto.NameOfProduct, result.NameOfProduct);
            Assert.Equal(crop.Id.ToString(), result.Crop.Id);
        }

        [Fact]
        public async Task AddFertilization_ThrowsException_WhenCropNotFound()
        {
            var fertilizationEditDto = new FertilizationEditDTO { CropId = Guid.NewGuid() };

            _cropRepositoryMock
                .Setup(repo => repo.GetById(fertilizationEditDto.CropId))
                .ReturnsAsync((Crop)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _fertilizationService.AddFertilization(fertilizationEditDto));
            Assert.Equal($"Crop not found with ID: {fertilizationEditDto.CropId}", exception.Message);
        }

        [Fact]
        public async Task GetFertilizationById_ReturnsDTO_WhenFertilizationExists()
        {
            var id = Guid.NewGuid();
            var crop = new Crop { Id = Guid.NewGuid(), Name = "Test Crop" };
            var fertilization = new Fertilization
            {
                Id = id,
                Crop = crop,
                Date = DateTime.Now,
                Type = FertilizationType.NotSelected,
                NameOfProduct = "Test Product",
                Quantity = 10.5,
                Description = "Test Description"
            };

            _fertilizationRepositoryMock
                .Setup(repo => repo.GetById(id))
                .ReturnsAsync(fertilization);

            var result = await _fertilizationService.GetFertilizationById(id);

            Assert.NotNull(result);
            Assert.Equal(fertilization.Id, result.Id);
            Assert.Equal(fertilization.NameOfProduct, result.NameOfProduct);
        }

        [Fact]
        public async Task GetFertilizationById_ThrowsException_WhenFertilizationNotFound()
        {
            var id = Guid.NewGuid();

            _fertilizationRepositoryMock
                .Setup(repo => repo.GetById(id))
                .ReturnsAsync((Fertilization)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _fertilizationService.GetFertilizationById(id));
            Assert.Equal($"Fertilization not found with ID: {id}", exception.Message);
        }

        [Fact]
        public async Task GetFertilizationsByCropId_ReturnsListOfDTOs_WhenFertilizationsExist()
        {
            var cropId = Guid.NewGuid();
            var crop = new Crop { Id = cropId, Name = "Test Crop" };
            var fertilizations = new List<Fertilization>
            {
                new Fertilization
                {
                    Id = Guid.NewGuid(),
                    Crop = crop,
                    Date = DateTime.Now,
                    Type = FertilizationType.NotSelected,
                    NameOfProduct = "Product 1",
                    Quantity = 5.0,
                    Description = "Description 1"
                },
                new Fertilization
                {
                    Id = Guid.NewGuid(),
                    Crop = crop,
                    Date = DateTime.Now,
                    Type = FertilizationType.NotSelected,
                    NameOfProduct = "Product 2",
                    Quantity = 10.0,
                    Description = "Description 2"
                }
            };

            _fertilizationRepositoryMock
                .Setup(repo => repo.GetFertilizationsByCropId(cropId))
                .ReturnsAsync(fertilizations);

            var result = await _fertilizationService.GetFertilizationsByCropId(cropId);

            Assert.NotNull(result);
            IEnumerable<FertilizationDTO> fertilizationDtos = result as FertilizationDTO[] ?? result.ToArray();
            Assert.Equal(fertilizations.Count, fertilizationDtos.Count());
            Assert.Contains(fertilizationDtos, f => f.NameOfProduct == "Product 1");
        }

        [Fact]
        public async Task DeleteFertilization_DeletesSuccessfully()
        {
            var id = Guid.NewGuid();

            _fertilizationRepositoryMock
                .Setup(repo => repo.Delete(id))
                .Returns(Task.CompletedTask);

            await _fertilizationService.DeleteFertilization(id);

            _fertilizationRepositoryMock.Verify(repo => repo.Delete(id), Times.Once);
        }

        [Fact]
        public async Task UpdateFertilization_ReturnsDTO_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var cropId = Guid.NewGuid();
            var crop = new Crop { Id = cropId, Name = "Test Crop" };
            var fertilization = new Fertilization
            {
                Id = id,
                Crop = crop,
                Date = DateTime.Now,
                Type = FertilizationType.NotSelected,
                NameOfProduct = "Old Product",
                Quantity = 5.0,
                Description = "Old Description"
            };

            var fertilizationEditDto = new FertilizationEditDTO
            {
                CropId = cropId,
                Date = DateTime.Now,
                Type = FertilizationType.NotSelected,
                NameOfProduct = "New Product",
                Quantity = 10.0,
                Description = "New Description"
            };

            _fertilizationRepositoryMock
                .Setup(repo => repo.GetById(id))
                .ReturnsAsync(fertilization);

            _cropRepositoryMock
                .Setup(repo => repo.GetById(cropId))
                .ReturnsAsync(crop);

            _fertilizationRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<Fertilization>()))
                .Returns(Task.CompletedTask);

            var result = await _fertilizationService.UpdateFertilization(id, fertilizationEditDto);

            Assert.NotNull(result);
            Assert.Equal(fertilizationEditDto.NameOfProduct, result.NameOfProduct);
            Assert.Equal(fertilizationEditDto.Description, result.Description);
        }
    }
}