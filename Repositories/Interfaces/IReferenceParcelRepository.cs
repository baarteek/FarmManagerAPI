using FarmManagerAPI.Models;

namespace FarmManagerAPI.Repositories.Interfaces
{
    public interface IReferenceParcelRepository : IGenericRepository<ReferenceParcel>
    {
        Task<IEnumerable<ReferenceParcel>> GetParcelsByFieldId(Guid fieldId);
    }
}
