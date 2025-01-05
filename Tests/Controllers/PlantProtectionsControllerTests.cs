using FarmManagerAPI.Controllers;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Controllers
{
    public class PlantProtectionsControllerTests
    {
        private readonly Mock<IPlantProtectionService> _plantProtectionServiceMock;
        private readonly PlantProtectionsController _controller;

        public PlantProtectionsControllerTests()
        {
            _plantProtectionServiceMock = new Mock<IPlantProtectionService>();
            _controller = new PlantProtectionsController(_plantProtectionServiceMock.Object);
        }

        [Fact]
        public async Task GetPlantProtection_ReturnsOk_WhenPlantProtectionExists()
        {
            var plantProtectionId = Guid.NewGuid();
            var plantProtectionDto = new PlantProtectionDTO
            {
                Id = plantProtectionId,
                Date = DateTime.UtcNow
            };

            _plantProtectionServiceMock
                .Setup(service => service.GetPlantProtectionById(plantProtectionId))
                .ReturnsAsync(plantProtectionDto);

            var result = await _controller.GetPlantProtection(plantProtectionId);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPlantProtection = Assert.IsType<PlantProtectionDTO>(actionResult.Value);
            Assert.Equal(plantProtectionId, returnedPlantProtection.Id);
        }

        [Fact]
        public async Task GetPlantProtection_ReturnsNotFound_WhenPlantProtectionDoesNotExist()
        {
            var plantProtectionId = Guid.NewGuid();

            _plantProtectionServiceMock
                .Setup(service => service.GetPlantProtectionById(plantProtectionId))
                .ThrowsAsync(new Exception("Plant protection not found"));

            var result = await _controller.GetPlantProtection(plantProtectionId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            dynamic errorMessage = notFoundResult.Value!;
            Assert.Equal("Plant protection not found", errorMessage.message);
        }

        [Fact]
        public async Task GetPlantProtectionsByCropId_ReturnsOk_WithPlantProtections()
        {
            var cropId = Guid.NewGuid();
            var plantProtections = new List<PlantProtectionDTO>
            {
                new PlantProtectionDTO { Id = Guid.NewGuid(), Date = DateTime.UtcNow },
                new PlantProtectionDTO { Id = Guid.NewGuid(), Date = DateTime.UtcNow }
            };

            _plantProtectionServiceMock
                .Setup(service => service.GetPlantProtectionsByCropId(cropId))
                .ReturnsAsync(plantProtections);

            var result = await _controller.GetPlantProtectionsByCropId(cropId);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPlantProtections = Assert.IsType<List<PlantProtectionDTO>>(actionResult.Value);
            Assert.Equal(plantProtections.Count, returnedPlantProtections.Count);
        }

        [Fact]
        public async Task AddPlantProtection_ReturnsCreatedAtAction_WhenSuccessful()
        {
            var plantProtectionEditDto = new PlantProtectionEditDTO
            {
                Date = DateTime.UtcNow
            };

            var plantProtectionDto = new PlantProtectionDTO
            {
                Id = Guid.NewGuid(),
                Date = plantProtectionEditDto.Date
            };

            _plantProtectionServiceMock
                .Setup(service => service.AddPlantProtection(plantProtectionEditDto))
                .ReturnsAsync(plantProtectionDto);

            var result = await _controller.AddPlantProtection(plantProtectionEditDto);
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedPlantProtection = Assert.IsType<PlantProtectionDTO>(actionResult.Value);
            Assert.Equal(plantProtectionDto.Id, returnedPlantProtection.Id);
        }

        [Fact]
        public async Task AddPlantProtection_ReturnsBadRequest_WhenExceptionThrown()
        {
            var plantProtectionEditDto = new PlantProtectionEditDTO
            {
                Date = DateTime.UtcNow
            };

            _plantProtectionServiceMock
                .Setup(service => service.AddPlantProtection(plantProtectionEditDto))
                .ThrowsAsync(new Exception("Error adding plant protection"));

            var result = await _controller.AddPlantProtection(plantProtectionEditDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            dynamic errorMessage = badRequestResult.Value!;
            Assert.Equal("Error adding plant protection", errorMessage.message);
        }

        [Fact]
        public async Task UpdatePlantProtection_ReturnsOk_WhenSuccessful()
        {
            var plantProtectionId = Guid.NewGuid();
            var plantProtectionEditDto = new PlantProtectionEditDTO
            {
                Date = DateTime.UtcNow
            };

            var plantProtectionDto = new PlantProtectionDTO
            {
                Id = plantProtectionId,
                Date = plantProtectionEditDto.Date
            };

            _plantProtectionServiceMock
                .Setup(service => service.UpdatePlantProtection(plantProtectionId, plantProtectionEditDto))
                .ReturnsAsync(plantProtectionDto);

            var result = await _controller.UpdatePlantProtection(plantProtectionId, plantProtectionEditDto);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPlantProtection = Assert.IsType<PlantProtectionDTO>(actionResult.Value);
            Assert.Equal(plantProtectionDto.Id, returnedPlantProtection.Id);
        }

        [Fact]
        public async Task UpdatePlantProtection_ReturnsBadRequest_WhenExceptionThrown()
        {
            var plantProtectionId = Guid.NewGuid();
            var plantProtectionEditDto = new PlantProtectionEditDTO
            {
                Date = DateTime.UtcNow
            };

            _plantProtectionServiceMock
                .Setup(service => service.UpdatePlantProtection(plantProtectionId, plantProtectionEditDto))
                .ThrowsAsync(new Exception("Error updating plant protection"));

            var result = await _controller.UpdatePlantProtection(plantProtectionId, plantProtectionEditDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            dynamic errorMessage = badRequestResult.Value!;
            Assert.Equal("Error updating plant protection", errorMessage.message);
        }

        [Fact]
        public async Task DeletePlantProtection_ReturnsNoContent_WhenSuccessful()
        {
            var plantProtectionId = Guid.NewGuid();

            _plantProtectionServiceMock
                .Setup(service => service.DeletePlantProtection(plantProtectionId))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeletePlantProtection(plantProtectionId);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void GetPlantProtectionTypes_ReturnsEnumValues()
        {
            var result = _controller.GetPlantProtectionTypes();
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var plantProtectionTypes = Assert.IsType<List<EnumResponse>>(actionResult.Value);
            Assert.NotEmpty(plantProtectionTypes);
            Assert.Contains(plantProtectionTypes, p => p.Name == PlantProtectionType.Herbicide.ToString());
        }
    }
}