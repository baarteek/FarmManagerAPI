using FarmManagerAPI.DTOs;

namespace FarmManagerAPI.Services.Interfaces
{
    public interface IFieldService
    {
        Task<FieldDTO> GetFieldById(Guid id);
        Task<IEnumerable<FieldDTO>> GetFieldsByFarmId(Guid farmId);
        Task<FieldDTO> AddField(FieldEditDTO fieldEditDto);
        Task<FieldDTO> UpdateField(Guid id, FieldEditDTO fieldEditDto);
        Task DeleteField(Guid id);
    }
}
