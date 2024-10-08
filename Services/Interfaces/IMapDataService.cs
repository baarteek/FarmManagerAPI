using FarmManagerAPI.DTOs;

namespace FarmManagerAPI.Services.Interfaces;

public interface IMapDataService
{
    Task<IEnumerable<MapDataDTO>> GetMapDataByFarmId(Guid farmId);
    Task<IEnumerable<MapDataDTO>> GetMapDataByUser(string userName);
}