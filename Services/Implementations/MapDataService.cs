using FarmManagerAPI.DTOs;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FarmManagerAPI.Services.Implementations; 

public class MapDataService : IMapDataService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IFieldRepository _fieldRepository;
    private readonly ICropRepository _cropRepository;

    public MapDataService(UserManager<IdentityUser> userManager,IFieldRepository fieldRepository, ICropRepository cropRepository)
    {
        _userManager = userManager;
        _fieldRepository = fieldRepository;
        _cropRepository = cropRepository;
    }
    
    public async Task<IEnumerable<MapDataDTO>> GetMapDataByFarmId(Guid farmId)
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

    public async Task<IEnumerable<MapDataDTO>> GetMapDataByUser(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new Exception($"User not found with username: {userName}");
        }
        
        var fields = (await _fieldRepository.GetFieldsByUser(user.Id)).ToList();
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