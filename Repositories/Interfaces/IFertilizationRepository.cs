using FarmManagerAPI.Models;

namespace FarmManagerAPI.Repositories.Interfaces
{
    public interface IFertilizationRepository : IGenericRepository<Fertilization>
    {
        Task<IEnumerable<Fertilization>> GetFertilizationsByCropId(Guid cropId);
    }
}