using FarmManagerAPI.DTOs;

namespace FarmManagerAPI.Services.Interfaces
{
    public interface IFertilizationService
    {
        Task<FertilizationDTO> GetFertilizationById(Guid id);
        Task<IEnumerable<FertilizationDTO>> GetFertilizationsByCropId(Guid cropId);
        Task<FertilizationDTO> AddFertilization(FertilizationEditDTO fertilizationEditDto);
        Task<FertilizationDTO> UpdateFertilization(Guid id, FertilizationEditDTO fertilizationEditDto);
        Task DeleteFertilization(Guid id);
    }
}
