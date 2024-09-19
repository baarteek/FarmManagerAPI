using FarmManagerAPI.DTOs;

namespace FarmManagerAPI.Services.Interfaces;

public interface IMapDataService
{
    Task<IEnumerable<MapDataDTO>> GetMapData(Guid farmId);
}