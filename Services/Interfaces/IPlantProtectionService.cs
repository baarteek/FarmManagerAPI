using FarmManagerAPI.DTOs;

namespace FarmManagerAPI.Services.Interfaces
{
    public interface IPlantProtectionService
    {
        Task<PlantProtectionDTO> AddPlantProtection(PlantProtectionEditDTO plantProtectionEditDto);
        Task DeletePlantProtection(Guid id);
        Task<PlantProtectionDTO> GetPlantProtectionById(Guid id);
        Task<IEnumerable<PlantProtectionDTO>> GetPlantProtectionsByCropId(Guid cropId);
        Task<PlantProtectionDTO> UpdatePlantProtection(Guid id, PlantProtectionEditDTO plantProtectionEditDto);
    }
}
