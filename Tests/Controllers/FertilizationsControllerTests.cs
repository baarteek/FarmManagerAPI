using FarmManagerAPI.Controllers;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Controllers
{
    public class FertilizationsControllerTests
    {
        private readonly Mock<IFertilizationService> _fertilizationServiceMock;
        private readonly FertilizationsController _controller;

        public FertilizationsControllerTests()
        {
            _fertilizationServiceMock = new Mock<IFertilizationService>();
            _controller = new FertilizationsController(_fertilizationServiceMock.Object);
        }

        [Fact]
        public async Task GetFertilization_ReturnsOk_WhenFertilizationExists()
        {
            var fertilizationId = Guid.NewGuid();
            var fertilizationDto = new FertilizationDTO
            {
                Id = fertilizationId,
                Date = DateTime.UtcNow,
                Type = FertilizationType.NotSelected,
            };

            _fertilizationServiceMock
                .Setup(service => service.GetFertilizationById(fertilizationId))
                .ReturnsAsync(fertilizationDto);

            var result = await _controller.GetFertilization(fertilizationId);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedFertilization = Assert.IsType<FertilizationDTO>(actionResult.Value);
            Assert.Equal(fertilizationId, returnedFertilization.Id);
        }

        [Fact]
        public async Task GetFertilization_ReturnsNotFound_WhenFertilizationDoesNotExist()
        {
            var fertilizationId = Guid.NewGuid();

            _fertilizationServiceMock
                .Setup(service => service.GetFertilizationById(fertilizationId))
                .ThrowsAsync(new Exception("Fertilization not found"));

            var result = await _controller.GetFertilization(fertilizationId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            dynamic errorMessage = notFoundResult.Value!;
            Assert.Equal("Fertilization not found", errorMessage.message);
        }

        [Fact]
        public async Task GetFertilizationsByCropId_ReturnsOk_WithFertilizations()
        {
            var cropId = Guid.NewGuid();
            var fertilizations = new List<FertilizationDTO>
            {
                new FertilizationDTO { Id = Guid.NewGuid(), Date = DateTime.UtcNow },
                new FertilizationDTO { Id = Guid.NewGuid(), Date = DateTime.UtcNow }
            };

            _fertilizationServiceMock
                .Setup(service => service.GetFertilizationsByCropId(cropId))
                .ReturnsAsync(fertilizations);

            var result = await _controller.GetFertilizationsByCropId(cropId);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedFertilizations = Assert.IsType<List<FertilizationDTO>>(actionResult.Value);
            Assert.Equal(fertilizations.Count, returnedFertilizations.Count);
        }

        [Fact]
        public async Task AddFertilization_ReturnsCreatedAtAction_WhenSuccessful()
        {
            var fertilizationEditDto = new FertilizationEditDTO
            {
                Date = DateTime.UtcNow
            };

            var fertilizationDto = new FertilizationDTO
            {
                Id = Guid.NewGuid(),
                Date = fertilizationEditDto.Date
            };

            _fertilizationServiceMock
                .Setup(service => service.AddFertilization(fertilizationEditDto))
                .ReturnsAsync(fertilizationDto);

            var result = await _controller.AddFertilization(fertilizationEditDto);
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedFertilization = Assert.IsType<FertilizationDTO>(actionResult.Value);
            Assert.Equal(fertilizationDto.Id, returnedFertilization.Id);
        }

        [Fact]
        public async Task AddFertilization_ReturnsBadRequest_WhenExceptionThrown()
        {
            var fertilizationEditDto = new FertilizationEditDTO
            {
                Date = DateTime.UtcNow
            };

            _fertilizationServiceMock
                .Setup(service => service.AddFertilization(fertilizationEditDto))
                .ThrowsAsync(new Exception("Error adding fertilization"));

            var result = await _controller.AddFertilization(fertilizationEditDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            dynamic errorMessage = badRequestResult.Value!;
            Assert.Equal("Error adding fertilization", errorMessage.message);
        }

        [Fact]
        public async Task UpdateFertilization_ReturnsOk_WhenSuccessful()
        {
            var fertilizationId = Guid.NewGuid();
            var fertilizationEditDto = new FertilizationEditDTO
            {
                Date = DateTime.UtcNow
            };

            var fertilizationDto = new FertilizationDTO
            {
                Id = fertilizationId,
                Date = fertilizationEditDto.Date
            };

            _fertilizationServiceMock
                .Setup(service => service.UpdateFertilization(fertilizationId, fertilizationEditDto))
                .ReturnsAsync(fertilizationDto);

            var result = await _controller.UpdateFertilization(fertilizationId, fertilizationEditDto);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedFertilization = Assert.IsType<FertilizationDTO>(actionResult.Value);
            Assert.Equal(fertilizationDto.Id, returnedFertilization.Id);
        }

        [Fact]
        public async Task UpdateFertilization_ReturnsBadRequest_WhenExceptionThrown()
        {
            var fertilizationId = Guid.NewGuid();
            var fertilizationEditDto = new FertilizationEditDTO
            {
                Date = DateTime.UtcNow
            };

            _fertilizationServiceMock
                .Setup(service => service.UpdateFertilization(fertilizationId, fertilizationEditDto))
                .ThrowsAsync(new Exception("Error updating fertilization"));

            var result = await _controller.UpdateFertilization(fertilizationId, fertilizationEditDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            dynamic errorMessage = badRequestResult.Value!;
            Assert.Equal("Error updating fertilization", errorMessage.message);
        }

        [Fact]
        public async Task DeleteFertilization_ReturnsNoContent_WhenSuccessful()
        {
            var fertilizationId = Guid.NewGuid();

            _fertilizationServiceMock
                .Setup(service => service.DeleteFertilization(fertilizationId))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteFertilization(fertilizationId);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void GetFertilizationTypes_ReturnsEnumValues()
        {
            var result = _controller.GetFertilizationTypes();
            
            var actionResult = Assert.IsType<OkObjectResult>(result);
            
            var fertilizationTypes = Assert.IsAssignableFrom<IEnumerable<EnumResponse>>(actionResult.Value);
            var fertilizationTypesList = fertilizationTypes.ToList();

            Assert.NotEmpty(fertilizationTypesList);
            Assert.Contains(fertilizationTypesList, f => f.Name == FertilizationType.NotSelected.ToString());
        }
    }
}