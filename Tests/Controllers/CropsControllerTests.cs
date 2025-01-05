using FarmManagerAPI.Controllers;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Controllers
{
    public class CropsControllerTests
    {
        private readonly Mock<ICropService> _cropServiceMock;
        private readonly CropsController _cropsController;

        public CropsControllerTests()
        {
            _cropServiceMock = new Mock<ICropService>();
            _cropsController = new CropsController(_cropServiceMock.Object);
        }

        [Fact]
        public async Task AddCrop_ReturnsCreatedAtAction_WhenSuccessful()
        {
            var cropEditDto = new CropEditDTO
            {
                Name = "Wheat",
                Type = CropType.NotSelected,
                IsActive = true
            };

            var cropDto = new CropDTO
            {
                Id = Guid.NewGuid(),
                Name = cropEditDto.Name,
                Type = cropEditDto.Type,
                IsActive = cropEditDto.IsActive,
            };

            _cropServiceMock
                .Setup(service => service.AddCrop(cropEditDto))
                .ReturnsAsync(cropDto);

            var result = await _cropsController.AddCrop(cropEditDto);
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedCrop = Assert.IsType<CropDTO>(actionResult.Value);
            Assert.Equal(cropDto.Id, returnedCrop.Id);
        }

        [Fact]
        public async Task GetCropById_ReturnsOk_WhenCropExists()
        {
            var cropDto = new CropDTO
            {
                Id = Guid.NewGuid(),
                Name = "Wheat",
                Type = CropType.NotSelected,
                IsActive = true
            };

            _cropServiceMock
                .Setup(service => service.GetCropById(cropDto.Id))
                .ReturnsAsync(cropDto);

            var result = await _cropsController.GetCropById(cropDto.Id);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCrop = Assert.IsType<CropDTO>(actionResult.Value);
            Assert.Equal(cropDto.Id, returnedCrop.Id);
        }

        [Fact]
        public async Task GetCropById_ReturnsNotFound_WhenCropDoesNotExist()
        {
            _cropServiceMock
                .Setup(service => service.GetCropById(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Crop not found"));

            var result = await _cropsController.GetCropById(Guid.NewGuid());
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            dynamic errorMessage = notFoundResult.Value!;
            Assert.Equal("Crop not found", errorMessage.message);
        }

        [Fact]
        public async Task GetActiveCropsByUser_ReturnsOk_WithCrops()
        {
            TestHelper.SetUser(_cropsController, "TestUser");

            var crops = new List<CropDTO>
            {
                new CropDTO { Id = Guid.NewGuid(), Name = "Corn", Type = CropType.NotSelected, IsActive = true },
                new CropDTO { Id = Guid.NewGuid(), Name = "Wheat", Type = CropType.Other, IsActive = false}
            };

            _cropServiceMock
                .Setup(service => service.GetActiveCropsByUser("TestUser"))
                .ReturnsAsync(crops);

            var result = await _cropsController.GetActiveCropsByUser();
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCrops = Assert.IsType<List<CropDTO>>(actionResult.Value);
            Assert.Equal(crops.Count, returnedCrops.Count);
        }

        [Fact]
        public async Task GetActiveCropsByUser_ReturnsUnauthorized_WhenUserNotFound()
        {
            TestHelper.SetNoUser(_cropsController);

            var result = await _cropsController.GetActiveCropsByUser();
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("User ID not found in token.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task GetCropsByUser_ReturnsOk_WithCrops()
        {
            TestHelper.SetUser(_cropsController, "TestUser");

            var crops = new List<CropDTO>
            {
                new CropDTO { Id = Guid.NewGuid(), Name = "Corn", Type = CropType.NotSelected, IsActive = true },
                new CropDTO { Id = Guid.NewGuid(), Name = "Wheat", Type = CropType.Other, IsActive = false}
            };

            _cropServiceMock
                .Setup(service => service.GetCropsByUser("TestUser"))
                .ReturnsAsync(crops);

            var result = await _cropsController.GetCropsByUser();
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCrops = Assert.IsType<List<CropDTO>>(actionResult.Value);
            Assert.Equal(crops.Count, returnedCrops.Count);
        }

        [Fact]
        public async Task GetCropsByUser_ReturnsUnauthorized_WhenUserNotFound()
        {
            TestHelper.SetNoUser(_cropsController);

            var result = await _cropsController.GetCropsByUser();
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("User ID not found in token.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task GetCropsByFieldId_ReturnsOk_WithCrops()
        {
            var fieldId = Guid.NewGuid();
            var crops = new List<CropDTO>
            {
                new CropDTO { Id = Guid.NewGuid(), Name = "Corn", Type = CropType.NotSelected, IsActive = true },
                new CropDTO { Id = Guid.NewGuid(), Name = "Wheat", Type = CropType.Other, IsActive = false}
            };

            _cropServiceMock
                .Setup(service => service.GetCropsByFieldId(fieldId))
                .ReturnsAsync(crops);

            var result = await _cropsController.GetCropsByFieldId(fieldId);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCrops = Assert.IsType<List<CropDTO>>(actionResult.Value);
            Assert.Equal(crops.Count, returnedCrops.Count);
        }

        [Fact]
        public async Task UpdateCrop_ReturnsNoContent_WhenSuccessful()
        {
            var cropId = Guid.NewGuid();
            var cropEditDto = new CropEditDTO
            {
                Name = "Wheat",
                Type = CropType.NotSelected,
                IsActive = true,
            };

            _cropServiceMock
                .Setup(service => service.UpdateCrop(cropId, cropEditDto))
                .Returns(Task.CompletedTask);

            var result = await _cropsController.UpdateCrop(cropId, cropEditDto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCrop_ReturnsNoContent_WhenSuccessful()
        {
            var cropId = Guid.NewGuid();

            _cropServiceMock
                .Setup(service => service.DeleteCrop(cropId))
                .Returns(Task.CompletedTask);

            var result = await _cropsController.DeleteCrop(cropId);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void GetCropTypes_ReturnsEnumValues()
        {
            var result = _cropsController.GetCropTypes();
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var cropTypes = Assert.IsType<List<EnumResponse>>(actionResult.Value);
            Assert.NotEmpty(cropTypes);
            Assert.Contains(cropTypes, c => c.Name == CropType.NotSelected.ToString());
        }

        [Fact]
        public async Task GetAgrotechnicalInterventions_ReturnsOk_WithInterventions()
        {
            var interventions = new List<EnumDTO>
            {
                new EnumDTO {  Name = "Plowing" },
                new EnumDTO {  Name = "Seeding",  }
            };

            _cropServiceMock
                .Setup(service => service.GetAgrotechnicalInterventions())
                .ReturnsAsync(interventions);

            var result = await _cropsController.GetAgrotechnicalInterventions();
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedInterventions = Assert.IsType<List<EnumDTO>>(actionResult.Value);
            Assert.Equal(interventions.Count, returnedInterventions.Count);
        }
    }
}