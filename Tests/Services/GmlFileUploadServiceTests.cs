using System.Text;
using System.Xml.Linq;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Implementations;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Services
{
    public class GmlFileUploadServiceTests
    {
        private readonly Mock<IFarmRepository> _farmRepositoryMock;
        private readonly Mock<IFieldRepository> _fieldRepositoryMock;
        private readonly Mock<ICropRepository> _cropRepositoryMock;
        private readonly GmlFileUploadService _gmlFileUploadService;

        public GmlFileUploadServiceTests()
        {
            _farmRepositoryMock = new Mock<IFarmRepository>();
            _fieldRepositoryMock = new Mock<IFieldRepository>();
            _cropRepositoryMock = new Mock<ICropRepository>();

            _gmlFileUploadService = new GmlFileUploadService(
                _farmRepositoryMock.Object,
                _fieldRepositoryMock.Object,
                _cropRepositoryMock.Object
            );
        }

        [Fact]
        public async Task ReadFileContent_ThrowsException_WhenFileIsEmpty()
        {
            var emptyFileMock = new Mock<IFormFile>();
            emptyFileMock.Setup(f => f.Length).Returns(0);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _gmlFileUploadService.ReadFileContent(emptyFileMock.Object, Guid.NewGuid()));

            Assert.Equal("No file provided or the file is empty.", exception.Message);
        }

        [Fact]
        public async Task ReadFileContent_ThrowsException_WhenFileHasInvalidExtension()
        {
            var invalidFileMock = new Mock<IFormFile>();
            invalidFileMock.Setup(f => f.FileName).Returns("test.txt");
            invalidFileMock.Setup(f => f.Length).Returns(100);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _gmlFileUploadService.ReadFileContent(invalidFileMock.Object, Guid.NewGuid()));

            Assert.Equal("Invalid file extension. Only .gml files are allowed.", exception.Message);
        }

        
        [Fact]
        public void ParseCoordinatesToGeoJson_ReturnsCorrectCoordinates()
        {
            var posList = "10 20 30 40 50 60";

            var result = _gmlFileUploadService.ParseCoordinatesToGeoJson(posList);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(3, result[0].Count);
            Assert.Equal(new List<double> { 10, 20 }, result[0][0]);
            Assert.Equal(new List<double> { 30, 40 }, result[0][1]);
            Assert.Equal(new List<double> { 50, 60 }, result[0][2]);
        }

        [Fact]
        public async Task ProcessCrop_CreatesNewFieldAndCrop_WhenFieldDoesNotExist()
        {
            var crop = XElement.Parse(@"
                <uprawa>
                    <roslina_uprawna>Wheat</roslina_uprawna>
                    <oznaczenie_uprawy>U001</oznaczenie_uprawy>
                    <powierzchnia>15.5</powierzchnia>
                    <gml:Polygon xmlns:gml='http://www.opengis.net/gml/3.2'>
                        <gml:posList>10 20 30 40 50 60</gml:posList>
                    </gml:Polygon>
                </uprawa>");

            var farmId = Guid.NewGuid();
            _fieldRepositoryMock
                .Setup(repo => repo.GetFieldIdByCoordinates(It.IsAny<string>()))
                .ReturnsAsync(Guid.Empty);

            _farmRepositoryMock
                .Setup(repo => repo.GetById(farmId))
                .ReturnsAsync(new Farm { Id = farmId, Name = "Test Farm" });

            _fieldRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<Field>()))
                .Returns(Task.CompletedTask);

            _cropRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<Crop>()))
                .Returns(Task.CompletedTask);

            await _gmlFileUploadService.ProcessCrop(crop, farmId);

            _fieldRepositoryMock.Verify(repo => repo.Add(It.IsAny<Field>()), Times.Once);
            _cropRepositoryMock.Verify(repo => repo.Add(It.IsAny<Crop>()), Times.Once);
        }
    }
}