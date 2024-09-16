using FarmManagerAPI.Models;

namespace FarmManagerAPI.Repositories.Interfaces
{
    public interface IFieldRepository : IGenericRepository<Field>
    {
        Task<IEnumerable<Field>> GetFieldsByFarmId(Guid farmId);
        Task<string> GetCoordinatesByFieldId(Guid fieldId);
    }
}
