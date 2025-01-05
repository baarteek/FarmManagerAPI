using System.Text;
using FarmManagerAPI.Controllers;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FarmManagerAPI.Tests.Controllers
{
    public class ReportControllerTests
    {
        private readonly Mock<IReportService> _reportServiceMock;
        private readonly ReportController _controller;

        public ReportControllerTests()
        {
            _reportServiceMock = new Mock<IReportService>();
            _controller = new ReportController(_reportServiceMock.Object);
        }

        [Fact]
        public async Task GetAgrotechnicalActivitiesReportHtml_ReturnsOk_WhenReportGenerated()
        {
            var farmId = Guid.NewGuid();
            var mockData = new List<AgrotechnicalActivitiesDTO>
            {
                new AgrotechnicalActivitiesDTO { CropIdentifier = "Crop1", PlotNumber = "1", Area = 100.0 },
                new AgrotechnicalActivitiesDTO { CropIdentifier = "Crop2", PlotNumber = "2", Area = 200.0 }
            };
            var mockHtml = "<html><body>Mock Report</body></html>";

            _reportServiceMock
                .Setup(service => service.GetAgrotechnicalActivitiesReportData(farmId))
                .ReturnsAsync(mockData);

            _reportServiceMock
                .Setup(service => service.GenerateAgrotechnicalActivitiesReportHtml(mockData))
                .Returns(mockHtml);

            var result = await _controller.GetAgrotechnicalActivitiesReportHtml(farmId);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedHtml = Assert.IsType<string>(actionResult.Value);
            Assert.Equal(mockHtml, returnedHtml);
        }

        [Fact]
        public async Task GetAgrotechnicalActivitiesReportHtml_ReturnsBadRequest_WhenExceptionThrown()
        {
            var farmId = Guid.NewGuid();

            _reportServiceMock
                .Setup(service => service.GetAgrotechnicalActivitiesReportData(farmId))
                .ThrowsAsync(new Exception("Error fetching report data"));

            var result = await _controller.GetAgrotechnicalActivitiesReportHtml(farmId);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Error fetching report data", badRequestResult.Value);
        }

        [Fact]
        public async Task GetAgrotechnicalActivitiesReportPdf_ReturnsPdfFile_WhenReportGenerated()
        {
            var farmId = Guid.NewGuid();
            var mockData = new List<AgrotechnicalActivitiesDTO>
            {
                new AgrotechnicalActivitiesDTO { CropIdentifier = "Crop1", PlotNumber = "1", Area = 100.0 },
                new AgrotechnicalActivitiesDTO { CropIdentifier = "Crop2", PlotNumber = "2", Area = 200.0 }
            };
            var mockHtml = "<html><body>Mock Report</body></html>";
            var mockPdf = Encoding.UTF8.GetBytes("Mock PDF Content");

            _reportServiceMock
                .Setup(service => service.GetAgrotechnicalActivitiesReportData(farmId))
                .ReturnsAsync(mockData);

            _reportServiceMock
                .Setup(service => service.GenerateAgrotechnicalActivitiesReportHtml(mockData))
                .Returns(mockHtml);

            _reportServiceMock
                .Setup(service => service.GenerateAgrotechnicalActivitiesReportPdf(mockHtml))
                .Returns(mockPdf);

            var result = await _controller.GetAgrotechnicalActivitiesReportPdf(farmId);
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/pdf", fileResult.ContentType);
            Assert.Equal(mockPdf, fileResult.FileContents);
            Assert.Contains("Report", fileResult.FileDownloadName);
        }

        [Fact]
        public async Task GetAgrotechnicalActivitiesReportPdf_ReturnsBadRequest_WhenExceptionThrown()
        {
            var farmId = Guid.NewGuid();

            _reportServiceMock
                .Setup(service => service.GetAgrotechnicalActivitiesReportData(farmId))
                .ThrowsAsync(new Exception("Error generating PDF report"));

            var result = await _controller.GetAgrotechnicalActivitiesReportPdf(farmId);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error generating PDF report", badRequestResult.Value);
        }

        [Fact]
        public async Task GetAgrotechnicalActivitiesReportExcelFromHtml_ReturnsExcelFile_WhenReportGenerated()
        {
            var farmId = Guid.NewGuid();
            var mockData = new List<AgrotechnicalActivitiesDTO>
            {
                new AgrotechnicalActivitiesDTO { CropIdentifier = "Crop1", PlotNumber = "1", Area = 100.0 },
                new AgrotechnicalActivitiesDTO { CropIdentifier = "Crop2", PlotNumber = "2", Area = 200.0 }
            };
            var mockHtml = "<html><body>Mock Report</body></html>";
            var mockBytes = Encoding.UTF8.GetBytes(mockHtml);

            _reportServiceMock
                .Setup(service => service.GetAgrotechnicalActivitiesReportData(farmId))
                .ReturnsAsync(mockData);

            _reportServiceMock
                .Setup(service => service.GenerateAgrotechnicalActivitiesReportHtml(mockData))
                .Returns(mockHtml);

            var result = await _controller.GetAgrotechnicalActivitiesReportExcelFromHtml(farmId);
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/vnd.ms-excel", fileResult.ContentType);
            Assert.Equal(mockBytes, fileResult.FileContents);
            Assert.Contains("Report", fileResult.FileDownloadName);
        }

        [Fact]
        public async Task GetAgrotechnicalActivitiesReportExcelFromHtml_ReturnsBadRequest_WhenExceptionThrown()
        {
            var farmId = Guid.NewGuid();

            _reportServiceMock
                .Setup(service => service.GetAgrotechnicalActivitiesReportData(farmId))
                .ThrowsAsync(new Exception("Error generating Excel report"));

            var result = await _controller.GetAgrotechnicalActivitiesReportExcelFromHtml(farmId);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error generating Excel report", badRequestResult.Value);
        }
    }
}