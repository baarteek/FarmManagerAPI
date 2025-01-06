using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Implementations;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Services
{
    public class ReferenceParcelServiceTests
    {
        private readonly Mock<IReferenceParcelRepository> _parcelRepositoryMock;
        private readonly Mock<IFieldRepository> _fieldRepositoryMock;
        private readonly ReferenceParcelService _referenceParcelService;

        public ReferenceParcelServiceTests()
        {
            _parcelRepositoryMock = new Mock<IReferenceParcelRepository>();
            _fieldRepositoryMock = new Mock<IFieldRepository>();
            _referenceParcelService = new ReferenceParcelService(
                _parcelRepositoryMock.Object,
                _fieldRepositoryMock.Object
            );
        }

        [Fact]
        public async Task AddParcel_ReturnsReferenceParcelDTO_WhenSuccessful()
        {
            var fieldId = Guid.NewGuid();
            var field = new Field { Id = fieldId, Name = "Test Field" };
            var parcelEditDto = new ReferenceParcelEditDTO
            {
                FieldId = fieldId,
                ParcelNumber = "Parcel-001",
                Area = 100.5
            };

            _fieldRepositoryMock
                .Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync(field);

            _parcelRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<ReferenceParcel>()))
                .Returns(Task.CompletedTask);

            var result = await _referenceParcelService.AddParcel(parcelEditDto);

            Assert.NotNull(result);
            Assert.Equal(parcelEditDto.ParcelNumber, result.ParcelNumber);
            Assert.Equal(field.Id.ToString(), result.Field.Id);
        }

        [Fact]
        public async Task AddParcel_ThrowsException_WhenFieldNotFound()
        {
            var parcelEditDto = new ReferenceParcelEditDTO { FieldId = Guid.NewGuid() };

            _fieldRepositoryMock
                .Setup(repo => repo.GetById(parcelEditDto.FieldId))
                .ReturnsAsync((Field)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _referenceParcelService.AddParcel(parcelEditDto));

            Assert.Equal($"Field not found with ID: {parcelEditDto.FieldId}", exception.Message);
        }

        [Fact]
        public async Task GetParcelById_ReturnsReferenceParcelDTO_WhenParcelExists()
        {
            var parcelId = Guid.NewGuid();
            var field = new Field { Id = Guid.NewGuid(), Name = "Test Field" };
            var parcel = new ReferenceParcel
            {
                Id = parcelId,
                Field = field,
                ParcelNumber = "Parcel-001",
                Area = 100.5
            };

            _parcelRepositoryMock
                .Setup(repo => repo.GetById(parcelId))
                .ReturnsAsync(parcel);

            var result = await _referenceParcelService.GetParcelById(parcelId);

            Assert.NotNull(result);
            Assert.Equal(parcel.ParcelNumber, result.ParcelNumber);
            Assert.Equal(field.Name, result.Field.Name);
        }

        [Fact]
        public async Task GetParcelsByFieldId_ReturnsListOfReferenceParcelDTOs_WhenParcelsExist()
        {
            var fieldId = Guid.NewGuid();
            var field = new Field { Id = fieldId, Name = "Test Field" };
            var parcels = new List<ReferenceParcel>
            {
                new ReferenceParcel { Id = Guid.NewGuid(), Field = field, ParcelNumber = "Parcel-001", Area = 100.5 },
                new ReferenceParcel { Id = Guid.NewGuid(), Field = field, ParcelNumber = "Parcel-002", Area = 200.0 }
            };

            _parcelRepositoryMock
                .Setup(repo => repo.GetParcelsByFieldId(fieldId))
                .ReturnsAsync(parcels);

            var result = await _referenceParcelService.GetParcelsByFieldId(fieldId);

            Assert.NotNull(result);
            IEnumerable<ReferenceParcelDTO> referenceParcelDtos = result as ReferenceParcelDTO[] ?? result.ToArray();
            Assert.Equal(parcels.Count, referenceParcelDtos.Count());
            Assert.Contains(referenceParcelDtos, p => p.ParcelNumber == "Parcel-001");
        }

        [Fact]
        public async Task UpdateParcel_UpdatesSuccessfully_WhenParcelExists()
        {
            var parcelId = Guid.NewGuid();
            var fieldId = Guid.NewGuid();
            var field = new Field { Id = fieldId, Name = "Test Field" };
            var parcel = new ReferenceParcel
            {
                Id = parcelId,
                Field = field,
                ParcelNumber = "Old Parcel",
                Area = 100.0
            };
            var parcelEditDto = new ReferenceParcelEditDTO
            {
                FieldId = fieldId,
                ParcelNumber = "Updated Parcel",
                Area = 150.0
            };

            _parcelRepositoryMock
                .Setup(repo => repo.GetById(parcelId))
                .ReturnsAsync(parcel);

            _fieldRepositoryMock
                .Setup(repo => repo.GetById(fieldId))
                .ReturnsAsync(field);

            _parcelRepositoryMock
                .Setup(repo => repo.Update(It.IsAny<ReferenceParcel>()))
                .Returns(Task.CompletedTask);

            await _referenceParcelService.UpdateParcel(parcelId, parcelEditDto);

            _parcelRepositoryMock.Verify(repo => repo.Update(It.Is<ReferenceParcel>(p =>
                p.ParcelNumber == parcelEditDto.ParcelNumber &&
                p.Area == parcelEditDto.Area)), Times.Once);
        }

        [Fact]
        public async Task UpdateParcel_ThrowsException_WhenParcelNotFound()
        {
            var parcelId = Guid.NewGuid();
            var parcelEditDto = new ReferenceParcelEditDTO { FieldId = Guid.NewGuid() };

            _parcelRepositoryMock
                .Setup(repo => repo.GetById(parcelId))
                .ReturnsAsync((ReferenceParcel)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _referenceParcelService.UpdateParcel(parcelId, parcelEditDto));

            Assert.Equal($"Parcel not found with ID: {parcelId}", exception.Message);
        }

        [Fact]
        public async Task DeleteParcel_DeletesSuccessfully_WhenParcelExists()
        {
            var parcelId = Guid.NewGuid();

            _parcelRepositoryMock
                .Setup(repo => repo.Delete(parcelId))
                .Returns(Task.CompletedTask);

            await _referenceParcelService.DeleteParcel(parcelId);

            _parcelRepositoryMock.Verify(repo => repo.Delete(parcelId), Times.Once);
        }
    }
}