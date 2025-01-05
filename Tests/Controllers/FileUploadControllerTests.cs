using FarmManagerAPI.Controllers;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Controllers
{
    public class FileUploadControllerTests
    {
        private readonly Mock<IGmlFileUploadService> _gmlFileUploadServiceMock;
        private readonly Mock<ICsvFileUploadService> _csvFileUploadServiceMock;
        private readonly FileUploadController _controller;

        public FileUploadControllerTests()
        {
            _gmlFileUploadServiceMock = new Mock<IGmlFileUploadService>();
            _csvFileUploadServiceMock = new Mock<ICsvFileUploadService>();
            _controller = new FileUploadController(_gmlFileUploadServiceMock.Object, _csvFileUploadServiceMock.Object);
        }

        private IFormFile CreateMockFile(string fileName, string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            return new FormFile(stream, 0, stream.Length, "file", fileName);
        }

        [Fact]
        public async Task UploadGmlFile_ReturnsOk_WhenFileIsProcessedSuccessfully()
        {
            var file = CreateMockFile("test.gml", "<gml>Test Content</gml>");
            var farmId = Guid.NewGuid();

            _gmlFileUploadServiceMock
                .Setup(service => service.ReadFileContent(file, farmId))
                .Returns(Task.CompletedTask);

            var result = await _controller.UploadGmlFile(file, farmId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value!;
            Assert.Equal("File uploaded and processed successfully", value.message);
            Assert.Equal(file.FileName, value.fileName);
        }

        [Fact]
        public async Task UploadGmlFile_ReturnsBadRequest_WhenFileIsEmpty()
        {
            var file = CreateMockFile("test.gml", "");
            var farmId = Guid.NewGuid();

            var result = await _controller.UploadGmlFile(file, farmId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file provided or the file is empty.", badRequestResult.Value);
        }

        [Fact]
        public async Task UploadGmlFile_ReturnsBadRequest_WhenArgumentExceptionIsThrown()
        {
            var file = CreateMockFile("test.gml", "<gml>Test Content</gml>");
            var farmId = Guid.NewGuid();

            _gmlFileUploadServiceMock
                .Setup(service => service.ReadFileContent(file, farmId))
                .ThrowsAsync(new ArgumentException("Invalid file format"));

            var result = await _controller.UploadGmlFile(file, farmId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid file format", badRequestResult.Value);
        }

        [Fact]
        public async Task UploadGmlFile_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var file = CreateMockFile("test.gml", "<gml>Test Content</gml>");
            var farmId = Guid.NewGuid();

            _gmlFileUploadServiceMock
                .Setup(service => service.ReadFileContent(file, farmId))
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.UploadGmlFile(file, farmId);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error: Unexpected error", statusCodeResult.Value);
        }

        [Fact]
        public async Task UploadCsvFile_ReturnsOk_WhenFileIsProcessedSuccessfully()
        {
            var file = CreateMockFile("test.csv", "Header1,Header2\nValue1,Value2");
            var farmId = Guid.NewGuid();

            _csvFileUploadServiceMock
                .Setup(service => service.ReadFileContent(file, farmId))
                .Returns(Task.CompletedTask);

            var result = await _controller.UploadCsvFile(file, farmId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            dynamic value = okResult.Value!;
            Assert.Equal("File uploaded and processed successfully", value.message);
            Assert.Equal(file.FileName, value.fileName);
        }

        [Fact]
        public async Task UploadCsvFile_ReturnsBadRequest_WhenFileIsEmpty()
        {
            var file = CreateMockFile("test.csv", "");
            var farmId = Guid.NewGuid();

            var result = await _controller.UploadCsvFile(file, farmId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file provided or the file is empty.", badRequestResult.Value);
        }

        [Fact]
        public async Task UploadCsvFile_ReturnsBadRequest_WhenArgumentExceptionIsThrown()
        {
            var file = CreateMockFile("test.csv", "Header1,Header2\nValue1,Value2");
            var farmId = Guid.NewGuid();

            _csvFileUploadServiceMock
                .Setup(service => service.ReadFileContent(file, farmId))
                .ThrowsAsync(new ArgumentException("Invalid CSV format"));

            var result = await _controller.UploadCsvFile(file, farmId);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid CSV format", badRequestResult.Value);
        }

        [Fact]
        public async Task UploadCsvFile_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            var file = CreateMockFile("test.csv", "Header1,Header2\nValue1,Value2");
            var farmId = Guid.NewGuid();

            _csvFileUploadServiceMock
                .Setup(service => service.ReadFileContent(file, farmId))
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.UploadCsvFile(file, farmId);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error: Unexpected error", statusCodeResult.Value);
        }
    }
}