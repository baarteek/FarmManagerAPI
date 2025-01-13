using FarmManagerAPI.Models;

namespace FarmManagerAPI.Repositories.Interfaces;

public interface ICultivationOperationRepository : IGenericRepository<CultivationOperation>
{
    Task<IEnumerable<CultivationOperation>> GetCultivationOperationsByCropId(Guid cropId);
    Task<CultivationOperation?> GetLatestCultivationOperationByUser(string userId);
}