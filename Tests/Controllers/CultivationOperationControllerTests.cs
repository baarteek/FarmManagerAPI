using FarmManagerAPI.Controllers;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Controllers
{
    public class CultivationOperationControllerTests
    {
        private readonly Mock<ICultivationOperationService> _cultivationOperationServiceMock;
        private readonly CultivationOperationController _controller;

        public CultivationOperationControllerTests()
        {
            _cultivationOperationServiceMock = new Mock<ICultivationOperationService>();
            _controller = new CultivationOperationController(_cultivationOperationServiceMock.Object);
        }

        [Fact]
        public async Task GetCultivationOperation_ReturnsOk_WhenOperationExists()
        {
            var operationId = Guid.NewGuid();
            var operationDto = new CultivationOperationDTO
            {
                Id = operationId,
                Name = "Plowing",
                Date = DateTime.UtcNow
            };

            _cultivationOperationServiceMock
                .Setup(service => service.GetCultivationOperationById(operationId))
                .ReturnsAsync(operationDto);

            var result = await _controller.GetCultivationOperation(operationId);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOperation = Assert.IsType<CultivationOperationDTO>(actionResult.Value);
            Assert.Equal(operationId, returnedOperation.Id);
        }

        [Fact]
        public async Task GetCultivationOperation_ReturnsBadRequest_WhenExceptionThrown()
        {
            var operationId = Guid.NewGuid();

            _cultivationOperationServiceMock
                .Setup(service => service.GetCultivationOperationById(operationId))
                .ThrowsAsync(new Exception("Operation not found"));

            var result = await _controller.GetCultivationOperation(operationId);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Operation not found", badRequestResult.Value);
        }

        [Fact]
        public async Task GetCultivationOperationsByCropId_ReturnsOk_WithOperations()
        {
            var cropId = Guid.NewGuid();
            var operations = new List<CultivationOperationDTO>
            {
                new CultivationOperationDTO { Id = Guid.NewGuid(), Name = "Plowing", Date = DateTime.UtcNow },
                new CultivationOperationDTO { Id = Guid.NewGuid(), Name = "Seeding", Date = DateTime.UtcNow }
            };

            _cultivationOperationServiceMock
                .Setup(service => service.GetCultivationOperationsByCropId(cropId))
                .ReturnsAsync(operations);

            var result = await _controller.GetCultivationOperationsByCropId(cropId);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOperations = Assert.IsType<List<CultivationOperationDTO>>(actionResult.Value);
            Assert.Equal(operations.Count, returnedOperations.Count);
        }

        [Fact]
        public async Task GetCultivationOperationsByCropId_ReturnsBadRequest_WhenExceptionThrown()
        {
            var cropId = Guid.NewGuid();

            _cultivationOperationServiceMock
                .Setup(service => service.GetCultivationOperationsByCropId(cropId))
                .ThrowsAsync(new Exception("Crop not found"));

            var result = await _controller.GetCultivationOperationsByCropId(cropId);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Crop not found", badRequestResult.Value);
        }

        [Fact]
        public async Task AddCultivationOperation_ReturnsOk_WhenSuccessful()
        {
            var operationEditDto = new CultivationOperationEditDTO
            {
                Name = "Plowing",
                Date = DateTime.UtcNow
            };

            _cultivationOperationServiceMock
                .Setup(service => service.AddCultivationOperation(operationEditDto))
                .Returns(Task.CompletedTask);

            var result = await _controller.AddCultivationOperation(operationEditDto);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task AddCultivationOperation_ReturnsBadRequest_WhenExceptionThrown()
        {
            var operationEditDto = new CultivationOperationEditDTO
            {
                Name = "Plowing",
                Date = DateTime.UtcNow
            };

            _cultivationOperationServiceMock
                .Setup(service => service.AddCultivationOperation(operationEditDto))
                .ThrowsAsync(new Exception("Error adding operation"));

            var result = await _controller.AddCultivationOperation(operationEditDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error adding operation", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateCultivationOperation_ReturnsNoContent_WhenSuccessful()
        {
            var operationId = Guid.NewGuid();
            var operationEditDto = new CultivationOperationEditDTO
            {
                Name = "Plowing",
                Date = DateTime.UtcNow
            };

            _cultivationOperationServiceMock
                .Setup(service => service.UpdateCultivationOperation(operationId, operationEditDto))
                .Returns(Task.CompletedTask);

            var result = await _controller.UpdateCultivationOperation(operationId, operationEditDto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateCultivationOperation_ReturnsBadRequest_WhenExceptionThrown()
        {
            var operationId = Guid.NewGuid();
            var operationEditDto = new CultivationOperationEditDTO
            {
                Name = "Plowing",
                Date = DateTime.UtcNow
            };

            _cultivationOperationServiceMock
                .Setup(service => service.UpdateCultivationOperation(operationId, operationEditDto))
                .ThrowsAsync(new Exception("Error updating operation"));

            var result = await _controller.UpdateCultivationOperation(operationId, operationEditDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error updating operation", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteCultivationOperation_ReturnsNoContent_WhenSuccessful()
        {
            var operationId = Guid.NewGuid();

            _cultivationOperationServiceMock
                .Setup(service => service.DeleteCultivationOperation(operationId))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteCultivationOperation(operationId);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCultivationOperation_ReturnsBadRequest_WhenExceptionThrown()
        {
            var operationId = Guid.NewGuid();

            _cultivationOperationServiceMock
                .Setup(service => service.DeleteCultivationOperation(operationId))
                .ThrowsAsync(new Exception("Error deleting operation"));

            var result = await _controller.DeleteCultivationOperation(operationId);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error deleting operation", badRequestResult.Value);
        }
    }
}