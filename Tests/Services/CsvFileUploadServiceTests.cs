using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Implementations;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Services
{
    public class CsvFileUploadServiceTests
    {
        private readonly Mock<IFieldRepository> _fieldRepositoryMock;
        private readonly Mock<ICropRepository> _cropRepositoryMock;
        private readonly Mock<IReferenceParcelRepository> _referenceParcelRepositoryMock;
        private readonly CsvFileUploadService _csvFileUploadService;

        public CsvFileUploadServiceTests()
        {
            _fieldRepositoryMock = new Mock<IFieldRepository>();
            _cropRepositoryMock = new Mock<ICropRepository>();
            _referenceParcelRepositoryMock = new Mock<IReferenceParcelRepository>();

            _csvFileUploadService = new CsvFileUploadService(
                _fieldRepositoryMock.Object,
                _cropRepositoryMock.Object,
                _referenceParcelRepositoryMock.Object
            );
        }

        [Fact]
        public async Task ReadFileContent_ThrowsException_WhenFileIsNullOrEmpty()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _csvFileUploadService.ReadFileContent(fileMock.Object, Guid.NewGuid()));

            Assert.Equal("No file provided or the file is empty.", exception.ParamName);
        }

        [Fact]
        public async Task ReadFileContent_ThrowsException_WhenFileHasInvalidExtension()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.Length).Returns(100);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _csvFileUploadService.ReadFileContent(fileMock.Object, Guid.NewGuid()));

            Assert.Equal("Invalid file extension. Only .csv files are allowed.", exception.Message);
        }
        

        [Fact]
        public async Task AddReferenceParcelToField_DoesNotAdd_WhenCropIdentifierIsEmpty()
        {
            var csvContent = @"
            Oznaczenie Uprawy / działki rolnej,Nr działki ewidencyjnej,Powierzchnia uprawy w granicach działki ewidencyjnej - ha
            ,Crop001,15.5";

            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.FileName).Returns("test.csv");

            _cropRepositoryMock
                .Setup(repo => repo.GetFieldIdByCropIdentifier(It.IsAny<string>()))
                .ReturnsAsync(Guid.Empty);

            await _csvFileUploadService.ReadFileContent(fileMock.Object, Guid.NewGuid());

            _referenceParcelRepositoryMock.Verify(repo => repo.Add(It.IsAny<ReferenceParcel>()), Times.Never);
        }
    }
}