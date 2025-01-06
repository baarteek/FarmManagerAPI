using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Implementations;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Services
{
    public class SoilMeasurementServiceTests
    {
        private readonly Mock<ISoilMeasurementRepository> _soilMeasurementRepositoryMock;
        private readonly Mock<IFieldRepository> _fieldRepositoryMock;
        private readonly SoilMeasurementService _soilMeasurementService;

        public SoilMeasurementServiceTests()
        {
            _soilMeasurementRepositoryMock = new Mock<ISoilMeasurementRepository>();
            _fieldRepositoryMock = new Mock<IFieldRepository>();
            _soilMeasurementService = new SoilMeasurementService(
                _soilMeasurementRepositoryMock.Object,
                _fieldRepositoryMock.Object
            );
        }

        [Fact]
        public async Task AddSoilMeasurement_ReturnsDTO_WhenSuccessful()
        {
            var fieldId = Guid.NewGuid();
            var field = new Field { Id = fieldId, Name = "Test Field" };
            var soilMeasurementEditDto = new SoilMeasurementEditDTO
            {
                FieldId = fieldId,
                Date = DateTime.Now,
                pH = 6.5,
                Nitrogen = 2.1,
                Phosphorus = 3.5,
                Potassium = 4.8
            };

            _fieldRepositoryMock
                .Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync(field);

            _soilMeasurementRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<SoilMeasurement>()))
                .Returns(Task.CompletedTask);

            var result = await _soilMeasurementService.AddSoilMeasurement(soilMeasurementEditDto);

            Assert.NotNull(result);
            Assert.Equal(soilMeasurementEditDto.pH, result.pH);
            Assert.Equal(field.Id.ToString(), result.Field.Id);
        }

        [Fact]
        public async Task AddSoilMeasurement_ThrowsException_WhenFieldNotFound()
        {
            var soilMeasurementEditDto = new SoilMeasurementEditDTO { FieldId = Guid.NewGuid() };

            _fieldRepositoryMock
                .Setup(repo => repo.GetById(soilMeasurementEditDto.FieldId))
                .ReturnsAsync((Field)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _soilMeasurementService.AddSoilMeasurement(soilMeasurementEditDto));

            Assert.Equal("Field not found", exception.Message);
        }

        [Fact]
        public async Task GetSoilMeasurementById_ReturnsDTO_WhenMeasurementExists()
        {
            var id = Guid.NewGuid();
            var field = new Field { Id = Guid.NewGuid(), Name = "Test Field" };
            var soilMeasurement = new SoilMeasurement
            {
                Id = id,
                Field = field,
                Date = DateTime.Now,
                pH = 6.5,
                Nitrogen = 2.1,
                Phosphorus = 3.5,
                Potassium = 4.8
            };

            _soilMeasurementRepositoryMock
                .Setup(repo => repo.GetById(id))
                .ReturnsAsync(soilMeasurement);

            var result = await _soilMeasurementService.GetSoilMeasurementById(id);

            Assert.NotNull(result);
            Assert.Equal(soilMeasurement.pH, result.pH);
            Assert.Equal(field.Name, result.Field.Name);
        }

        [Fact]
        public async Task GetSoilMeasurementById_ThrowsException_WhenMeasurementNotFound()
        {
            var id = Guid.NewGuid();

            _soilMeasurementRepositoryMock
                .Setup(repo => repo.GetById(id))
                .ReturnsAsync((SoilMeasurement)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _soilMeasurementService.GetSoilMeasurementById(id));

            Assert.Equal($"Soil Measurement not found with ID: {id}", exception.Message);
        }

        [Fact]
        public async Task GetSoilMeasurementsByFieldId_ReturnsListOfDTOs_WhenMeasurementsExist()
        {
            var fieldId = Guid.NewGuid();
            var field = new Field { Id = fieldId, Name = "Test Field" };
            var soilMeasurements = new List<SoilMeasurement>
            {
                new SoilMeasurement { Id = Guid.NewGuid(), Field = field, Date = DateTime.Now, pH = 6.5, Nitrogen = 2.1, Phosphorus = 3.5, Potassium = 4.8 },
                new SoilMeasurement { Id = Guid.NewGuid(), Field = field, Date = DateTime.Now, pH = 7.0, Nitrogen = 1.9, Phosphorus = 2.8, Potassium = 3.9 }
            };

            _soilMeasurementRepositoryMock
                .Setup(repo => repo.GetSoilMeasurementsByFieldId(fieldId))
                .ReturnsAsync(soilMeasurements);

            var result = await _soilMeasurementService.GetSoilMeasurementsByFieldId(fieldId);

            Assert.NotNull(result);
            IEnumerable<SoilMeasurementDTO> soilMeasurementDtos = result as SoilMeasurementDTO[] ?? result.ToArray();
            Assert.Equal(soilMeasurements.Count, soilMeasurementDtos.Count());
            Assert.Contains(soilMeasurementDtos, sm => sm.pH == 6.5);
        }

        [Fact]
        public async Task UpdateSoilMeasurement_ReturnsDTO_WhenSuccessful()
        {
            var id = Guid.NewGuid();
            var fieldId = Guid.NewGuid();
            var field = new Field { Id = fieldId, Name = "Test Field" };
            var soilMeasurement = new SoilMeasurement
            {
                Id = id,
                Field = field,
                Date = DateTime.Now,
                pH = 6.0,
                Nitrogen = 2.0,
                Phosphorus = 3.0,
                Potassium = 4.0
            };
            var soilMeasurementEditDto = new SoilMeasurementEditDTO
            {
                FieldId = fieldId,
                Date = DateTime.Now,
                pH = 6.5,
                Nitrogen = 2.1,
                Phosphorus = 3.5,
                Potassium = 4.8
            };

            _soilMeasurementRepositoryMock
                .Setup(repo => repo.GetById(id))
                .ReturnsAsync(soilMeasurement);

            _fieldRepositoryMock
                .Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync(field);

            _soilMeasurementRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<SoilMeasurement>()))
                .Returns(Task.CompletedTask);

            var result = await _soilMeasurementService.UpdateSoilMeasurement(id, soilMeasurementEditDto);

            Assert.NotNull(result);
            Assert.Equal(soilMeasurementEditDto.pH, result.pH);
            Assert.Equal(soilMeasurementEditDto.Potassium, result.Potassium);
        }

        [Fact]
        public async Task UpdateSoilMeasurement_ThrowsException_WhenMeasurementOrFieldNotFound()
        {
            var id = Guid.NewGuid();
            var soilMeasurementEditDto = new SoilMeasurementEditDTO { FieldId = Guid.NewGuid() };

            _soilMeasurementRepositoryMock
                .Setup(repo => repo.GetById(id))
                .ReturnsAsync((SoilMeasurement)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _soilMeasurementService.UpdateSoilMeasurement(id, soilMeasurementEditDto));

            Assert.Equal("Soil Measurement or Field not found", exception.Message);
        }

        [Fact]
        public async Task DeleteSoilMeasurement_DeletesSuccessfully()
        {
            var id = Guid.NewGuid();

            _soilMeasurementRepositoryMock
                .Setup(repo => repo.Delete(id))
                .Returns(Task.CompletedTask);

            await _soilMeasurementService.DeleteSoilMeasurement(id);

            _soilMeasurementRepositoryMock.Verify(repo => repo.Delete(id), Times.Once);
        }
    }
}