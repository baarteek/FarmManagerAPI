using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;

namespace FarmManagerAPI.Services.Interfaces
{
    public interface IReferenceParcelService
    {
        Task<ReferenceParcelDTO> AddParcel(ReferenceParcelEditDTO parcelEditDto);
        Task<ReferenceParcelDTO> GetParcelById(Guid id);
        Task<IEnumerable<ReferenceParcelDTO>> GetParcelsByFieldId(Guid fieldId);
        Task UpdateParcel(Guid id, ReferenceParcelEditDTO parcelEditDto);
        Task DeleteParcel(Guid id);
    }
}
