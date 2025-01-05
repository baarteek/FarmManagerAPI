using FarmManagerAPI.Controllers;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Controllers;

public class FieldsControllerTests
{
    private readonly Mock<IFieldService> _fieldServiceMock;
    private readonly FieldsController _fieldsController;

    public FieldsControllerTests()
    {
        _fieldServiceMock = new Mock<IFieldService>();
        _fieldsController = new FieldsController(_fieldServiceMock.Object);
    }

    [Fact]
    public async Task AddFields_ReturnsOk()
    {
        var newField = new FieldEditDTO
        {
            FarmId = Guid.NewGuid(),
            Name = "TestField",
            Area = 1.0,
            SoilType = 0,
        };

        var addedField = new FieldDTO
        {
            Id = Guid.NewGuid(),
            Name = newField.Name,
            Area = newField.Area,
            SoilType = newField.SoilType.ToString(),
        };

        _fieldServiceMock
            .Setup(service => service.AddField(newField))
            .ReturnsAsync(addedField);

        var result = await _fieldsController.AddField(newField);
        var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedField = Assert.IsType<FieldDTO>(actionResult.Value);
        Assert.Equal(addedField.Id, returnedField.Id);
        Assert.Equal(addedField.Name, returnedField.Name);
        Assert.Equal(addedField.Area, returnedField.Area);
        Assert.Equal(addedField.SoilType, returnedField.SoilType);
    }

    [Fact]
    public async Task AddFields_ReturnsBadRequest_WhenExceptionIsThrown()
    {
        var fieldEditDto = new FieldEditDTO
        {
            FarmId = Guid.NewGuid(),
            Name = "TestField",
            Area = 1.0,
            SoilType = 0,
        };
        
        _fieldServiceMock
            .Setup(service => service.AddField(fieldEditDto))
            .ThrowsAsync(new Exception("Error adding field"));
        
        var result = await _fieldsController.AddField(fieldEditDto);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        dynamic errorMessage = badRequestResult.Value!;
        Assert.NotNull(errorMessage);
        Assert.Equal("Error adding field", errorMessage.message);
    }

    [Fact]
        public async Task GetFieldById_ReturnsOk_WhenFieldExists()
        {
            var fieldDto = new FieldDTO
            {
                Id = Guid.NewGuid(),
                Name = "TestField",
                Area = 1.0,
                SoilType = SoilType.NotSelected.ToString(),
            };

            _fieldServiceMock
                .Setup(service => service.GetFieldById(fieldDto.Id))
                .ReturnsAsync(fieldDto);

            var result = await _fieldsController.GetFieldById(fieldDto.Id);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedField = Assert.IsType<FieldDTO>(actionResult.Value);
            Assert.Equal(fieldDto.Id, returnedField.Id);
            Assert.Equal(fieldDto.Name, returnedField.Name);
        }

        [Fact]
        public async Task GetFieldById_ReturnsNotFound_WhenFieldDoesNotExist()
        {
            _fieldServiceMock
                .Setup(service => service.GetFieldById(It.IsAny<Guid>()))
                .ReturnsAsync((FieldDTO)null!);

            var result = await _fieldsController.GetFieldById(Guid.NewGuid());
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            dynamic errorMessage = notFoundResult.Value!;
            Assert.Equal("Field not found", errorMessage.message);
        }

        [Fact]
        public async Task GetFarmsCoordinates_ReturnsOk_WhenCoordinatesExist()
        {
            var fieldId = Guid.NewGuid();
            var coordinates = "50.12345,19.67890";

            _fieldServiceMock
                .Setup(service => service.GetCoordinatesByFieldId(fieldId))
                .ReturnsAsync(coordinates);

            var result = await _fieldsController.GetFarmsCoordinates(fieldId);
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(coordinates, actionResult.Value);
        }

        [Fact]
        public async Task GetFarmsCoordinates_ReturnsNoContent_WhenCoordinatesDoNotExist()
        {
            var fieldId = Guid.NewGuid();

            _fieldServiceMock
                .Setup(service => service.GetCoordinatesByFieldId(fieldId))
                .ReturnsAsync(string.Empty);

            var result = await _fieldsController.GetFarmsCoordinates(fieldId);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetFieldsByFarmId_ReturnsFields()
        {
            var farmId = Guid.NewGuid();
            var fields = new List<FieldDTO>
            {
                new FieldDTO { Id = Guid.NewGuid(), Name = "Field1", Area = 2.0, SoilType = SoilType.NotSelected.ToString() },
                new FieldDTO { Id = Guid.NewGuid(), Name = "Field2", Area = 3.0, SoilType = SoilType.NotSelected.ToString() }
            };

            _fieldServiceMock
                .Setup(service => service.GetFieldsByFarmId(farmId))
                .ReturnsAsync(fields);

            var result = await _fieldsController.GetFieldsByFarmId(farmId);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedFields = Assert.IsType<List<FieldDTO>>(actionResult.Value);
            Assert.Equal(fields.Count, returnedFields.Count);
        }

        [Fact]
        public async Task UpdateField_ReturnsNoContent_WhenSuccessful()
        {
            var fieldId = Guid.NewGuid();
            var fieldEditDto = new FieldEditDTO
            {
                Name = "UpdatedField",
                Area = 5.0,
                SoilType = SoilType.NotSelected
            };

            var updatedField = new FieldDTO
            {
                Id = fieldId,
                Name = fieldEditDto.Name,
                Area = fieldEditDto.Area,
                SoilType = fieldEditDto.SoilType.ToString()
            };

            _fieldServiceMock
                .Setup(service => service.UpdateField(fieldId, fieldEditDto))
                .ReturnsAsync(updatedField);

            var result = await _fieldsController.UpdateField(fieldId, fieldEditDto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateField_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            var fieldId = Guid.NewGuid();
            var fieldEditDto = new FieldEditDTO
            {
                Name = "UpdatedField",
                Area = 5.0,
                SoilType = SoilType.NotSelected
            };

            _fieldServiceMock
                .Setup(service => service.UpdateField(fieldId, fieldEditDto))
                .ThrowsAsync(new Exception("Error updating field"));

            var result = await _fieldsController.UpdateField(fieldId, fieldEditDto);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            dynamic errorMessage = badRequestResult.Value!;
            Assert.Equal("Error updating field", errorMessage.message);
        }

        [Fact]
        public async Task DeleteField_ReturnsNoContent_WhenSuccessful()
        {
            var fieldId = Guid.NewGuid();

            _fieldServiceMock
                .Setup(service => service.DeleteField(fieldId))
                .Returns(Task.CompletedTask);

            var result = await _fieldsController.DeleteField(fieldId);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteField_ReturnsNotFound_WhenExceptionIsThrown()
        {
            var fieldId = Guid.NewGuid();

            _fieldServiceMock
                .Setup(service => service.DeleteField(fieldId))
                .ThrowsAsync(new Exception("Error deleting field"));

            var result = await _fieldsController.DeleteField(fieldId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            dynamic errorMessage = notFoundResult.Value!;
            Assert.Equal("Error deleting field", errorMessage.message);
        }

        [Fact]
        public void GetSoilTypes_ReturnsEnumValues()
        {
            var result = _fieldsController.GetSoilTypes();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var soilTypes = Assert.IsType<List<EnumResponse>>(okResult.Value);
            Assert.NotEmpty(soilTypes);
            Assert.Contains(soilTypes, s => s.Name == SoilType.Sandy.ToString());
        }

        [Fact]
        public async Task GetFieldsNameAndId_ReturnsFields()
        {
            var farmId = Guid.NewGuid();
            var fields = new List<MiniItemDTO>
            {
                new MiniItemDTO { Id = Guid.NewGuid().ToString(), Name = "Field1" },
                new MiniItemDTO { Id = Guid.NewGuid().ToString(), Name = "Field2" }
            };

            _fieldServiceMock
                .Setup(service => service.GetFieldsNamesAndId(farmId))
                .ReturnsAsync(fields);

            var result = await _fieldsController.GetFieldsNameAndId(farmId);
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedFields = Assert.IsType<List<MiniItemDTO>>(actionResult.Value);
            Assert.Equal(fields.Count, returnedFields.Count);
        }
}