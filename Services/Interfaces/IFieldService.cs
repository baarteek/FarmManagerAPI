using FarmManagerAPI.DTOs;

namespace FarmManagerAPI.Services.Interfaces
{
    public interface IFieldService
    {
        Task<FieldDTO> GetFieldById(Guid id);
        Task<IEnumerable<FieldDTO>> GetFieldsByFarmId(Guid farmId);
        Task<string> GetCoordinatesByFieldId(Guid fieldId);
        Task<FieldDTO> AddField(FieldEditDTO fieldEditDto);
        Task<FieldDTO> UpdateField(Guid id, FieldEditDTO fieldEditDto);
        Task DeleteField(Guid id);
        Task<IEnumerable<MiniItemDTO>> GetFieldsNamesAndId(Guid farmId);
    }
}
