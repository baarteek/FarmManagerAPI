using FarmManagerAPI.DTOs;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace FarmManagerAPI.Services.Implementations; 

public class MapDataService : IMapDataService
{
    private readonly IFieldRepository _fieldRepository;
    private readonly ICropRepository _cropRepository;

    public MapDataService(IFieldRepository fieldRepository, ICropRepository cropRepository)
    {
        _fieldRepository = fieldRepository;
        _cropRepository = cropRepository;
    }
    
    public async Task<IEnumerable<MapDataDTO>> GetMapData(Guid farmId)
    {
        var fields = (await _fieldRepository.GetFieldsByFarmId(farmId)).ToList();
        if (fields.IsNullOrEmpty())
        {
            return new List<MapDataDTO>();
        }

        var mapDataList = new List<MapDataDTO>();
        foreach (var field in fields)
        {
            var crop = await _cropRepository.GetActiveCropByFieldId(field.Id);
            if (crop != null)
            {
                var dto = new MapDataDTO
                {
                    FieldId = field.Id,
                    FieldName = field.Name,
                    Area = field.Area,
                    Coordinates = field.Coordinates,
                    CropId = crop.Id,
                    CropName = crop.Name,
                };
                mapDataList.Add(dto);
            }
        }
        return mapDataList;
    }
}