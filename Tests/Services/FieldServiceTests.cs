using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Implementations;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Services
{
    public class FieldServiceTests
    {
        private readonly Mock<IFieldRepository> _fieldRepositoryMock;
        private readonly Mock<IFarmRepository> _farmRepositoryMock;
        private readonly Mock<IReferenceParcelRepository> _referenceParcelRepositoryMock;
        private readonly Mock<ISoilMeasurementRepository> _soilMeasurementRepositoryMock;
        private readonly Mock<ICropRepository> _cropRepositoryMock;
        private readonly FieldService _fieldService;

        public FieldServiceTests()
        {
            _fieldRepositoryMock = new Mock<IFieldRepository>();
            _farmRepositoryMock = new Mock<IFarmRepository>();
            _referenceParcelRepositoryMock = new Mock<IReferenceParcelRepository>();
            _soilMeasurementRepositoryMock = new Mock<ISoilMeasurementRepository>();
            _cropRepositoryMock = new Mock<ICropRepository>();

            _fieldService = new FieldService(
                _fieldRepositoryMock.Object,
                _farmRepositoryMock.Object,
                _referenceParcelRepositoryMock.Object,
                _soilMeasurementRepositoryMock.Object,
                _cropRepositoryMock.Object
            );
        }

        [Fact]
        public async Task AddField_ReturnsFieldDTO_WhenSuccessful()
        {
            var farmId = Guid.NewGuid();
            var farm = new Farm { Id = farmId, Name = "Test Farm" };
            var fieldEditDto = new FieldEditDTO
            {
                Name = "Test Field",
                Area = 100,
                SoilType = SoilType.Sandy,
                FarmId = farmId
            };

            _farmRepositoryMock
                .Setup(repo => repo.GetById(farmId))
                .ReturnsAsync(farm);

            _fieldRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<Field>()))
                .Returns(Task.CompletedTask);

            var result = await _fieldService.AddField(fieldEditDto);

            Assert.NotNull(result);
            Assert.Equal(fieldEditDto.Name, result.Name);
            Assert.Equal(farm.Id.ToString(), result.Farm.Id);
        }

        [Fact]
        public async Task AddField_ThrowsException_WhenFarmNotFound()
        {
            var fieldEditDto = new FieldEditDTO { FarmId = Guid.NewGuid() };

            _farmRepositoryMock
                .Setup(repo => repo.GetById(fieldEditDto.FarmId))
                .ReturnsAsync((Farm)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _fieldService.AddField(fieldEditDto));

            Assert.Equal("Farm not found", exception.Message);
        }

        [Fact]
        public async Task GetFieldById_ReturnsFieldDTO_WhenFieldExists()
        {
            var fieldId = Guid.NewGuid();
            var farm = new Farm { Id = Guid.NewGuid(), Name = "Test Farm" };
            var field = new Field
            {
                Id = fieldId,
                Name = "Test Field",
                Area = 100,
                SoilType = SoilType.Sandy,
                Farm = farm
            };

            _fieldRepositoryMock
                .Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync(field);

            _referenceParcelRepositoryMock
                .Setup(repo => repo.GetParcelsByFieldId(fieldId))
                .ReturnsAsync(new List<ReferenceParcel>());

            _soilMeasurementRepositoryMock
                .Setup(repo => repo.GetSoilMeasurementsByFieldId(fieldId))
                .ReturnsAsync(new List<SoilMeasurement>());

            _cropRepositoryMock
                .Setup(repo => repo.GetCropsByFieldId(fieldId))
                .ReturnsAsync(new List<Crop>());

            var result = await _fieldService.GetFieldById(fieldId);

            Assert.NotNull(result);
            Assert.Equal(field.Name, result.Name);
            Assert.Equal(field.Farm.Name, result.Farm.Name);
        }

        [Fact]
        public async Task GetFieldById_ThrowsException_WhenFieldNotFound()
        {
            var fieldId = Guid.NewGuid();

            _fieldRepositoryMock
                .Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync((Field)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _fieldService.GetFieldById(fieldId));

            Assert.Equal("Field not found", exception.Message);
        }

        [Fact]
        public async Task DeleteField_DeletesSuccessfully_WhenFieldExists()
        {
            var fieldId = Guid.NewGuid();
            var field = new Field { Id = fieldId, Name = "Test Field" };

            _fieldRepositoryMock
                .Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync(field);

            _fieldRepositoryMock
                .Setup(repo => repo.Delete(fieldId))
                .Returns(Task.CompletedTask);

            await _fieldService.DeleteField(fieldId);

            _fieldRepositoryMock.Verify(repo => repo.Delete(fieldId), Times.Once);
        }

        [Fact]
        public async Task DeleteField_ThrowsException_WhenFieldNotFound()
        {
            var fieldId = Guid.NewGuid();

            _fieldRepositoryMock
                .Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync((Field)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _fieldService.DeleteField(fieldId));

            Assert.Equal("Field not found", exception.Message);
        }

        [Fact]
        public async Task GetFieldsByFarmId_ReturnsFields_WhenFieldsExist()
        {
            var farmId = Guid.NewGuid();
            var farm = new Farm { Id = farmId, Name = "Test Farm" };
            var fields = new List<Field>
            {
                new Field
                {
                    Id = Guid.NewGuid(),
                    Name = "Field 1",
                    Area = 100,
                    SoilType = SoilType.Sandy,
                    Farm = farm
                },
                new Field
                {
                    Id = Guid.NewGuid(),
                    Name = "Field 2",
                    Area = 200,
                    SoilType = SoilType.Clay,
                    Farm = farm
                }
            };

            _fieldRepositoryMock
                .Setup(repo => repo.GetFieldsByFarmId(farmId))
                .ReturnsAsync(fields);

            var result = await _fieldService.GetFieldsByFarmId(farmId);

            Assert.NotNull(result);
            IEnumerable<FieldDTO> fieldDtos = result as FieldDTO[] ?? result.ToArray();
            Assert.Equal(fields.Count, fieldDtos.Count());
            Assert.Contains(fieldDtos, f => f.Name == "Field 1");
        }

        [Fact]
        public async Task UpdateField_ReturnsUpdatedDTO_WhenSuccessful()
        {
            var fieldId = Guid.NewGuid();
            var farmId = Guid.NewGuid();
            var farm = new Farm { Id = farmId, Name = "Test Farm" };
            var field = new Field
            {
                Id = fieldId,
                Name = "Old Field",
                Area = 100,
                SoilType = SoilType.Sandy,
                Farm = farm
            };

            var fieldEditDto = new FieldEditDTO
            {
                Name = "Updated Field",
                Area = 150,
                SoilType = SoilType.Clay,
                FarmId = farmId
            };

            _fieldRepositoryMock
                .Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync(field);

            _farmRepositoryMock
                .Setup(repo => repo.GetById(farmId))
                .ReturnsAsync(farm);

            _fieldRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<Field>()))
                .Returns(Task.CompletedTask);

            var result = await _fieldService.UpdateField(fieldId, fieldEditDto);

            Assert.NotNull(result);
            Assert.Equal(fieldEditDto.Name, result.Name);
            Assert.Equal(fieldEditDto.Area, result.Area);
        }
    }
}