using FarmManagerAPI.Models;

namespace FarmManagerAPI.Repositories.Interfaces
{
    public interface IFieldRepository : IGenericRepository<Field>
    {
        Task<IEnumerable<Field>> GetFieldsByFarmId(Guid farmId);
        Task<IEnumerable<Field>> GetFieldsByUser(string userId);
        Task<string> GetCoordinatesByFieldId(Guid fieldId);
        Task<Guid> GetFieldIdByCoordinates(string coordinates);
    }
}
