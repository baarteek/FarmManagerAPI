using FarmManagerAPI.DTOs;

namespace FarmManagerAPI.Services.Interfaces;

public interface ICultivationOperationService
{
    Task<CultivationOperationDTO> GetCultivationOperationById(Guid id);
    Task<IEnumerable<CultivationOperationDTO>> GetCultivationOperationsByCropId(Guid cropId);
    Task AddCultivationOperation(CultivationOperationEditDTO operation);
    Task UpdateCultivationOperation(Guid id, CultivationOperationEditDTO operation);
    Task DeleteCultivationOperation(Guid id);
}