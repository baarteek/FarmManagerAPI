using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;

namespace FarmManagerAPI.Services.Interfaces
{
    public interface ICropService
    {
        Task<CropDTO> GetCropById(Guid id);
        Task<IEnumerable<CropDTO>> GetCropsByFieldId(Guid fieldId);
        Task<IEnumerable<CropDTO>> GetCropsByUser(string userName);
        Task<IEnumerable<CropDTO>> GetActiveCropsByUser(string userName);
        Task<CropDTO> AddCrop(CropEditDTO cropEditDTO);
        Task UdpateCrop(Guid id, CropEditDTO cropEditDTO);
        Task DeleteCrop(Guid id);
    }
}
