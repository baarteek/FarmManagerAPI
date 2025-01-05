using FarmManagerAPI.Controllers;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FarmManagerAPI.Tests.Controllers
{
    public class ReferenceParcelsControllerTests
    {
        private readonly Mock<IReferenceParcelService> _parcelServiceMock;
        private readonly ReferenceParcelsController _parcelsController;

        public ReferenceParcelsControllerTests()
        {
            _parcelServiceMock = new Mock<IReferenceParcelService>();
            _parcelsController = new ReferenceParcelsController(_parcelServiceMock.Object);
        }

        [Fact]
        public async Task AddParcel_ReturnsCreatedAtAction_WhenSuccessful()
        {
            var parcelEditDto = new ReferenceParcelEditDTO
            {
                FieldId = Guid.NewGuid(),
                Area = 5.0,
                ParcelNumber = "testParcelNumber",
            };

            var parcelDto = new ReferenceParcelDTO
            {
                Id = Guid.NewGuid(),
                Area = parcelEditDto.Area,
                ParcelNumber = parcelEditDto.ParcelNumber,
            };

            _parcelServiceMock
                .Setup(service => service.AddParcel(parcelEditDto))
                .ReturnsAsync(parcelDto);

            var result = await _parcelsController.AddParcel(parcelEditDto);
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedParcel = Assert.IsType<ReferenceParcelDTO>(actionResult.Value);
            Assert.Equal(parcelDto.Id, returnedParcel.Id);
        }

        [Fact]
        public async Task GetParcelById_ReturnsOk_WhenParcelExists()
        {
            var parcelDto = new ReferenceParcelDTO
            {
                Id = Guid.NewGuid(),
                Area = 5.0,
                ParcelNumber = "testParcelNumber",
            };

            _parcelServiceMock
                .Setup(service => service.GetParcelById(parcelDto.Id))
                .ReturnsAsync(parcelDto);

            var result = await _parcelsController.GetParcelById(parcelDto.Id);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedParcel = Assert.IsType<ReferenceParcelDTO>(actionResult.Value);
            Assert.Equal(parcelDto.Id, returnedParcel.Id);
        }

        [Fact]
        public async Task GetParcelById_ReturnsNotFound_WhenParcelDoesNotExist()
        {
            _parcelServiceMock
                .Setup(service => service.GetParcelById(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Parcel not found"));

            var result = await _parcelsController.GetParcelById(Guid.NewGuid());
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            dynamic errorMessage = notFoundResult.Value!;
            Assert.Equal("Parcel not found", errorMessage.message);
        }

        [Fact]
        public async Task GetParcelsByFieldId_ReturnsOk_WithParcels()
        {
            var fieldId = Guid.NewGuid();
            var parcels = new List<ReferenceParcelDTO>
            {
                new ReferenceParcelDTO { Id = Guid.NewGuid(), Area = 5.0, ParcelNumber = "testParcelNumber1", },
                new ReferenceParcelDTO { Id = Guid.NewGuid(), Area = 6.0, ParcelNumber = "testParcelNumber2", },
            };

            _parcelServiceMock
                .Setup(service => service.GetParcelsByFieldId(fieldId))
                .ReturnsAsync(parcels);

            var result = await _parcelsController.GetParcelsByFieldId(fieldId);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedParcels = Assert.IsType<List<ReferenceParcelDTO>>(actionResult.Value);
            Assert.Equal(parcels.Count, returnedParcels.Count);
        }

        [Fact]
        public async Task UpdateParcel_ReturnsNoContent_WhenSuccessful()
        {
            var parcelId = Guid.NewGuid();
            var parcelEditDto = new ReferenceParcelEditDTO
            {
                FieldId = Guid.NewGuid(),
                Area = 5.5,
                ParcelNumber = "testParcelNumber",
            };

            _parcelServiceMock
                .Setup(service => service.UpdateParcel(parcelId, parcelEditDto))
                .Returns(Task.CompletedTask);

            var result = await _parcelsController.UpdateParcel(parcelId, parcelEditDto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateParcel_ReturnsNotFound_WhenExceptionIsThrown()
        {
            var parcelId = Guid.NewGuid();
            var parcelEditDto = new ReferenceParcelEditDTO
            {
                FieldId = Guid.NewGuid(),
                Area = 5.5,
                ParcelNumber = "testParcelNumber",
            };

            _parcelServiceMock
                .Setup(service => service.UpdateParcel(parcelId, parcelEditDto))
                .ThrowsAsync(new Exception("Parcel not found"));

            var result = await _parcelsController.UpdateParcel(parcelId, parcelEditDto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            dynamic errorMessage = notFoundResult.Value!;
            Assert.Equal("Parcel not found", errorMessage.message);
        }

        [Fact]
        public async Task DeleteParcel_ReturnsNoContent_WhenSuccessful()
        {
            var parcelId = Guid.NewGuid();

            _parcelServiceMock
                .Setup(service => service.DeleteParcel(parcelId))
                .Returns(Task.CompletedTask);

            var result = await _parcelsController.DeleteParcel(parcelId);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteParcel_ReturnsNotFound_WhenExceptionIsThrown()
        {
            var parcelId = Guid.NewGuid();

            _parcelServiceMock
                .Setup(service => service.DeleteParcel(parcelId))
                .ThrowsAsync(new Exception("Parcel not found"));

            var result = await _parcelsController.DeleteParcel(parcelId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            dynamic errorMessage = notFoundResult.Value!;
            Assert.Equal("Parcel not found", errorMessage.message);
        }
    }
}