using FarmManagerAPI.Models;

namespace FarmManagerAPI.Repositories.Interfaces
{
    public interface ICropRepository : IGenericRepository<Crop>
    {
        Task<IEnumerable<Crop>> GetCropsByFieldId(Guid fieldId);
    }
}
