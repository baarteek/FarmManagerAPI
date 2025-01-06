using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Implementations;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Services
{
    public class CultivationOperationServiceTests
    {
        private readonly Mock<ICultivationOperationRepository> _cultivationOperationRepositoryMock;
        private readonly Mock<ICropRepository> _cropRepositoryMock;
        private readonly CultivationOperationService _service;

        public CultivationOperationServiceTests()
        {
            _cultivationOperationRepositoryMock = new Mock<ICultivationOperationRepository>();
            _cropRepositoryMock = new Mock<ICropRepository>();
            _service = new CultivationOperationService(
                _cultivationOperationRepositoryMock.Object,
                _cropRepositoryMock.Object
            );
        }

        [Fact]
        public async Task GetCultivationOperationById_ReturnsDTO_WhenOperationExists()
        {
            var id = Guid.NewGuid();
            var crop = new Crop { Id = Guid.NewGuid(), Name = "Test Crop" };
            var operation = new CultivationOperation
            {
                Id = id,
                Name = "Test Operation",
                Date = DateTime.Now,
                AgrotechnicalIntervention = AgrotechnicalIntervention.None,
                Description = "Test Description",
                Crop = crop
            };

            _cultivationOperationRepositoryMock
                .Setup(repo => repo.GetById(id))
                .ReturnsAsync(operation);

            var result = await _service.GetCultivationOperationById(id);

            Assert.NotNull(result);
            Assert.Equal(operation.Id, result.Id);
            Assert.Equal(operation.Name, result.Name);
            Assert.Equal(operation.Crop.Name, result.Crop.Name);
        }

        [Fact]
        public async Task GetCultivationOperationById_ThrowsException_WhenOperationNotFound()
        {
            var id = Guid.NewGuid();

            _cultivationOperationRepositoryMock
                .Setup(repo => repo.GetById(id))
                .ReturnsAsync((CultivationOperation)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _service.GetCultivationOperationById(id));
            Assert.Equal($"Could not find cultivation operation with id: {id}", exception.Message);
        }

        [Fact]
        public async Task AddCultivationOperation_AddsOperation_WhenCropExists()
        {
            var cropId = Guid.NewGuid();
            var crop = new Crop { Id = cropId, Name = "Test Crop" };
            var operationDto = new CultivationOperationEditDTO
            {
                Name = "New Operation",
                Date = DateTime.Now,
                AgrotechnicalIntervention = AgrotechnicalIntervention.None,
                Description = "Description",
                CropId = cropId
            };

            _cropRepositoryMock
                .Setup(repo => repo.GetById(cropId))
                .ReturnsAsync(crop);

            _cultivationOperationRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<CultivationOperation>()))
                .Returns(Task.CompletedTask);

            await _service.AddCultivationOperation(operationDto);

            _cultivationOperationRepositoryMock.Verify(repo => repo.Add(It.Is<CultivationOperation>(op =>
                op.Name == operationDto.Name &&
                op.Crop.Id == cropId &&
                op.AgrotechnicalIntervention == operationDto.AgrotechnicalIntervention)), Times.Once);
        }

        [Fact]
        public async Task AddCultivationOperation_ThrowsException_WhenCropNotFound()
        {
            var operationDto = new CultivationOperationEditDTO { CropId = Guid.NewGuid() };

            _cropRepositoryMock
                .Setup(repo => repo.GetById(operationDto.CropId))
                .ReturnsAsync((Crop)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _service.AddCultivationOperation(operationDto));
            Assert.Equal($"Crop not found with ID: {operationDto.CropId}", exception.Message);
        }

        [Fact]
        public async Task DeleteCultivationOperation_DeletesOperation_WhenOperationExists()
        {
            var id = Guid.NewGuid();

            _cultivationOperationRepositoryMock
                .Setup(repo => repo.Delete(id))
                .Returns(Task.CompletedTask);

            await _service.DeleteCultivationOperation(id);

            _cultivationOperationRepositoryMock.Verify(repo => repo.Delete(id), Times.Once);
        }

        [Fact]
        public async Task GetCultivationOperationsByCropId_ReturnsOperations_WhenCropHasOperations()
        {
            var cropId = Guid.NewGuid();
            var crop = new Crop { Id = cropId, Name = "Test Crop" };
            var operations = new List<CultivationOperation>
            {
                new CultivationOperation
                {
                    Id = Guid.NewGuid(),
                    Name = "Operation 1",
                    Crop = crop,
                    Date = DateTime.Now
                },
                new CultivationOperation
                {
                    Id = Guid.NewGuid(),
                    Name = "Operation 2",
                    Crop = crop,
                    Date = DateTime.Now
                }
            };

            _cultivationOperationRepositoryMock
                .Setup(repo => repo.GetCultivationOperationsByCropId(cropId))
                .ReturnsAsync(operations);

            var result = await _service.GetCultivationOperationsByCropId(cropId);

            Assert.NotNull(result);
            Assert.Equal(operations.Count, result.Count());
        }
    }
}