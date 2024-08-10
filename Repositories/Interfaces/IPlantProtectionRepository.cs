using FarmManagerAPI.Models;

namespace FarmManagerAPI.Repositories.Interfaces
{
    public interface IPlantProtectionRepository : IGenericRepository<PlantProtection>
    {
        Task<IEnumerable<PlantProtection>> GetPlantProtectionsByCropId(Guid cropId);
    }
}