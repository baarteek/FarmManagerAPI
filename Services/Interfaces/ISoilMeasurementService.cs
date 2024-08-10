using FarmManagerAPI.DTOs;

namespace FarmManagerAPI.Services.Interfaces
{
    public interface ISoilMeasurementService
    {
        Task<SoilMeasurementDTO> AddSoilMeasurement(SoilMeasurementEditDTO soilMeasurementEditDto);
        Task DeleteSoilMeasurement(Guid id);
        Task<SoilMeasurementDTO> GetSoilMeasurementById(Guid id);
        Task<IEnumerable<SoilMeasurementDTO>> GetSoilMeasurementsByFieldId(Guid fieldId);
        Task<SoilMeasurementDTO> UpdateSoilMeasurement(Guid id, SoilMeasurementEditDTO soilMeasurementEditDto);
    }
}