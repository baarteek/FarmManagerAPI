using FarmManagerAPI.Models;

namespace FarmManagerAPI.Repositories.Interfaces
{
    public interface ICropRepository : IGenericRepository<Crop>
    {
        Task<IEnumerable<Crop>> GetCropsByFieldId(Guid fieldId);
        Task<IEnumerable<Crop>> GetCropsByUserId(string userId);
        Task<IEnumerable<Crop>> GetActiveCropsByUserId(string userId);
        Task<Crop> GetActiveCropByFieldId(Guid fieldId);
        Task<IEnumerable<Crop>> GetActiveCropsByFieldId(Guid fieldId);
        Task<Guid> GetCropIdByIdentifierAndFieldId(string cropIdentifier, Guid fieldId);
    }
}
