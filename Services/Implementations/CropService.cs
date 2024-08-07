using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;

namespace FarmManagerAPI.Services.Implementations
{
    public class CropService : ICropService
    {
        private readonly ICropRepository _cropRepository;

        public CropService(ICropRepository cropRepository)
        {
            _cropRepository = cropRepository;
        }

        public Task<Crop> AddCrop(CropEditDTO cropEditDTO)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCrop(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<CropDTO> GetCropById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CropDTO>> GetCropsByFieldId(Guid fieldId)
        {
            throw new NotImplementedException();
        }

        public Task UdpateCrop(CropEditDTO cropEditDTO)
        {
            throw new NotImplementedException();
        }
    }
}
