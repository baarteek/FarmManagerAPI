using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
namespace FarmManagerAPI.Services.Interfaces
{
    public interface IFarmService
    {
        Task<FarmDTO> GetFarmById(Guid id);
        Task<IEnumerable<FarmDTO>> GetFarmsByUser(string userId);
        Task<FarmDTO> AddFarm(FarmEditDTO farmEditDto, string userId);
        Task UpdateFarm(Guid id, FarmEditDTO farmEditDto);
        Task DeleteFarm(Guid id);
        Task<IEnumerable<MiniItemDTO>> GetFarmsNamesAndIdByUser(string userName);
    }
}
