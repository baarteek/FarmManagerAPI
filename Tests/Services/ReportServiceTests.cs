using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Implementations;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Services
{
    public class ReportServiceTests
    {
        private readonly Mock<IFarmRepository> _farmRepositoryMock;
        private readonly Mock<IFieldRepository> _fieldRepositoryMock;
        private readonly Mock<IReferenceParcelRepository> _referenceParcelRepositoryMock;
        private readonly Mock<ICropRepository> _cropRepositoryMock;
        private readonly Mock<ICultivationOperationRepository> _operationRepositoryMock;
        private readonly Mock<IPlantProtectionRepository> _plantProtectionRepositoryMock;
        private readonly Mock<IFertilizationRepository> _fertilizationRepositoryMock;
        private readonly ReportService _reportService;

        public ReportServiceTests()
        {
            _farmRepositoryMock = new Mock<IFarmRepository>();
            _fieldRepositoryMock = new Mock<IFieldRepository>();
            _referenceParcelRepositoryMock = new Mock<IReferenceParcelRepository>();
            _cropRepositoryMock = new Mock<ICropRepository>();
            _operationRepositoryMock = new Mock<ICultivationOperationRepository>();
            _plantProtectionRepositoryMock = new Mock<IPlantProtectionRepository>();
            _fertilizationRepositoryMock = new Mock<IFertilizationRepository>();

            _reportService = new ReportService(
                _farmRepositoryMock.Object,
                _fieldRepositoryMock.Object,
                _referenceParcelRepositoryMock.Object,
                _cropRepositoryMock.Object,
                _operationRepositoryMock.Object,
                _plantProtectionRepositoryMock.Object,
                _fertilizationRepositoryMock.Object
            );
        }

        [Fact]
        public async Task GetAgrotechnicalActivitiesReportData_ReturnsData_WhenFarmAndFieldsExist()
        {
            var farmId = Guid.NewGuid();
            var farm = new Farm { Id = farmId, Name = "Test Farm" };
            var fields = new List<Field>
            {
                new Field { Id = Guid.NewGuid(), Name = "Field 1" }
            };
            var referenceParcels = new List<ReferenceParcel>
            {
                new ReferenceParcel { Id = Guid.NewGuid(), ParcelNumber = "Parcel-001", Area = 10.5 }
            };
            var crop = new Crop { Id = Guid.NewGuid(), Name = "Crop 1", CropIdentifier = "C1" };
            var cultivationOperations = new List<CultivationOperation>
            {
                new CultivationOperation { Id = Guid.NewGuid(), Name = "Cultivation 1", Date = DateTime.Now, Description = "Desc 1" }
            };

            _farmRepositoryMock
                .Setup(repo => repo.GetById(farmId))
                .ReturnsAsync(farm);

            _fieldRepositoryMock
                .Setup(repo => repo.GetFieldsByFarmId(farmId))
                .ReturnsAsync(fields);

            _referenceParcelRepositoryMock
                .Setup(repo => repo.GetParcelsByFieldId(fields[0].Id))
                .ReturnsAsync(referenceParcels);

            _cropRepositoryMock
                .Setup(repo => repo.GetActiveCropByFieldId(fields[0].Id))
                .ReturnsAsync(crop);

            _operationRepositoryMock
                .Setup(repo => repo.GetCultivationOperationsByCropId(crop.Id))
                .ReturnsAsync(cultivationOperations);

            var result = await _reportService.GetAgrotechnicalActivitiesReportData(farmId);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("C1", result[0].CropIdentifier);
        }

        [Fact]
        public async Task GetAgrotechnicalActivitiesReportData_ThrowsException_WhenFarmNotFound()
        {
            var farmId = Guid.NewGuid();

            _farmRepositoryMock
                .Setup(repo => repo.GetById(farmId))
                .ReturnsAsync((Farm)null!);

            var exception = await Assert.ThrowsAsync<Exception>(() => _reportService.GetAgrotechnicalActivitiesReportData(farmId));
            Assert.Equal("Farm not found", exception.Message);
        }

        [Fact]
        public void GenerateAgrotechnicalActivitiesReportHtml_ReturnsHtmlContent()
        {
            var activities = new List<AgrotechnicalActivitiesDTO>
            {
                new AgrotechnicalActivitiesDTO
                {
                    CropIdentifier = "C1",
                    PlotNumber = "Parcel-001",
                    Date = DateTime.Now,
                    Area = 10.5,
                    TypeOfUse = "Crop 1",
                    TypeOfActivity = "Cultivation",
                    NameOfPlantProtectionProduct = "Product 1",
                    AmountOfPlatnProtectionProduct = "5 l/ha",
                    PackageNumber = "P1",
                    Comments = "Comment 1"
                }
            };

            var result = _reportService.GenerateAgrotechnicalActivitiesReportHtml(activities);

            Assert.NotNull(result);
            Assert.Contains("<html>", result);
            Assert.Contains("WYKAZ DZIAŁAŃ AGROTECHNICZNYCH", result);
            Assert.Contains("C1", result);
        }
    }
}