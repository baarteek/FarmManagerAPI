using FarmManagerAPI.Controllers;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Controllers
{
    public class SoilMeasurementsControllerTests
    {
        private readonly Mock<ISoilMeasurementService> _soilMeasurementServiceMock;
        private readonly SoilMeasurementsController _soilMeasurementsController;

        public SoilMeasurementsControllerTests()
        {
            _soilMeasurementServiceMock = new Mock<ISoilMeasurementService>();
            _soilMeasurementsController = new SoilMeasurementsController(_soilMeasurementServiceMock.Object);
        }

        [Fact]
        public async Task GetSoilMeasurementById_ReturnsOk_WhenSoilMeasurementExists()
        {
            var soilMeasurement = new SoilMeasurementDTO
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                Potassium = 100.0,
                Phosphorus = 90.0,
                pH = 5.0
            };

            _soilMeasurementServiceMock
                .Setup(service => service.GetSoilMeasurementById(soilMeasurement.Id))
                .ReturnsAsync(soilMeasurement);

            var result = await _soilMeasurementsController.GetSoilMeasurementById(soilMeasurement.Id);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedSoilMeasurement = Assert.IsType<SoilMeasurementDTO>(actionResult.Value);
            Assert.Equal(soilMeasurement.Id, returnedSoilMeasurement.Id);
        }

        [Fact]
        public async Task GetSoilMeasurementById_ReturnsNotFound_WhenSoilMeasurementDoesNotExist()
        {
            _soilMeasurementServiceMock
                .Setup(service => service.GetSoilMeasurementById(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Soil measurement not found"));

            var result = await _soilMeasurementsController.GetSoilMeasurementById(Guid.NewGuid());
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            dynamic errorMessage = notFoundResult.Value!;
            Assert.Equal("Soil measurement not found", errorMessage.message);
        }

        [Fact]
        public async Task GetSoilMeasurementsByFieldId_ReturnsOk_WithMeasurements()
        {
            var fieldId = Guid.NewGuid();
            var soilMeasurements = new List<SoilMeasurementDTO>
            {
                new SoilMeasurementDTO { Id = Guid.NewGuid(), pH = 6.5, Potassium = 80 },
                new SoilMeasurementDTO { Id = Guid.NewGuid(), pH = 7.2, Potassium = 120 }
            };

            _soilMeasurementServiceMock
                .Setup(service => service.GetSoilMeasurementsByFieldId(fieldId))
                .ReturnsAsync(soilMeasurements);

            var result = await _soilMeasurementsController.GetSoilMeasurementsByFieldId(fieldId);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedMeasurements = Assert.IsType<List<SoilMeasurementDTO>>(actionResult.Value);
            Assert.Equal(soilMeasurements.Count, returnedMeasurements.Count);
        }

        [Fact]
        public async Task AddSoilMeasurement_ReturnsCreated_WhenSuccessful()
        {
            var soilMeasurementEditDto = new SoilMeasurementEditDTO
            {
                FieldId = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                pH = 6.8,
                Potassium = 90
            };

            var soilMeasurementDto = new SoilMeasurementDTO
            {
                Id = Guid.NewGuid(),
                Date = soilMeasurementEditDto.Date,
                pH = soilMeasurementEditDto.pH,
                Potassium = soilMeasurementEditDto.Potassium
            };

            _soilMeasurementServiceMock
                .Setup(service => service.AddSoilMeasurement(soilMeasurementEditDto))
                .ReturnsAsync(soilMeasurementDto);

            var result = await _soilMeasurementsController.AddSoilMeasurement(soilMeasurementEditDto);
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedSoilMeasurement = Assert.IsType<SoilMeasurementDTO>(actionResult.Value);
            Assert.Equal(soilMeasurementDto.Id, returnedSoilMeasurement.Id);
        }

        [Fact]
        public async Task AddSoilMeasurement_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            var soilMeasurementEditDto = new SoilMeasurementEditDTO
            {
                FieldId = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                pH = 6.8,
                Potassium = 90
            };

            _soilMeasurementServiceMock
                .Setup(service => service.AddSoilMeasurement(soilMeasurementEditDto))
                .ThrowsAsync(new Exception("Error adding soil measurement"));

            var result = await _soilMeasurementsController.AddSoilMeasurement(soilMeasurementEditDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            dynamic errorMessage = badRequestResult.Value!;
            Assert.Equal("Error adding soil measurement", errorMessage.message);
        }

        [Fact]
        public async Task UpdateSoilMeasurement_ReturnsOk_WhenSuccessful()
        {
            var soilMeasurementEditDto = new SoilMeasurementEditDTO
            {
                FieldId = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                pH = 6.9,
                Potassium = 95
            };

            var updatedSoilMeasurement = new SoilMeasurementDTO
            {
                Id = Guid.NewGuid(),
                Date = soilMeasurementEditDto.Date,
                pH = soilMeasurementEditDto.pH,
                Potassium = soilMeasurementEditDto.Potassium
            };

            _soilMeasurementServiceMock
                .Setup(service => service.UpdateSoilMeasurement(updatedSoilMeasurement.Id, soilMeasurementEditDto))
                .ReturnsAsync(updatedSoilMeasurement);

            var result = await _soilMeasurementsController.UpdateSoilMeasurement(updatedSoilMeasurement.Id, soilMeasurementEditDto);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedSoilMeasurement = Assert.IsType<SoilMeasurementDTO>(actionResult.Value);
            Assert.Equal(updatedSoilMeasurement.Id, returnedSoilMeasurement.Id);
        }

        [Fact]
        public async Task DeleteSoilMeasurement_ReturnsNoContent_WhenSuccessful()
        {
            var soilMeasurementId = Guid.NewGuid();

            _soilMeasurementServiceMock
                .Setup(service => service.DeleteSoilMeasurement(soilMeasurementId))
                .Returns(Task.CompletedTask);

            var result = await _soilMeasurementsController.DeleteSoilMeasurement(soilMeasurementId);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteSoilMeasurement_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            var soilMeasurementId = Guid.NewGuid();

            _soilMeasurementServiceMock
                .Setup(service => service.DeleteSoilMeasurement(soilMeasurementId))
                .ThrowsAsync(new Exception("Error deleting soil measurement"));

            var result = await _soilMeasurementsController.DeleteSoilMeasurement(soilMeasurementId);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            dynamic errorMessage = badRequestResult.Value!;
            Assert.Equal("Error deleting soil measurement", errorMessage.message);
        }
    }
}