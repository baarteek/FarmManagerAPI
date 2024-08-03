using FarmManagerAPI.Models;

namespace FarmManagerAPI.Repositories.Interfaces
{
    public interface IFarmRepository : IGenericRepository<Farm>
    {
        Task<IEnumerable<Farm>> GetFarmsByUser(string userId);
    }
}
