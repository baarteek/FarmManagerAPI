using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;

namespace FarmManagerAPI.Services.Implementations
{
    public class FertilizationService : IFertilizationService
    {
        private readonly IFertilizationRepository _fertilizationRepository;
        private readonly ICropRepository _cropRepository;

        public FertilizationService(IFertilizationRepository fertilizationRepository, ICropRepository cropRepository)
        {
            _fertilizationRepository = fertilizationRepository;
            _cropRepository = cropRepository;
        }

        public async Task<FertilizationDTO> AddFertilization(FertilizationEditDTO fertilizationEditDto)
        {
            var crop = await _cropRepository.GetById(fertilizationEditDto.CropId);
            if (crop == null)
            {
                throw new Exception($"Crop not found with ID: {fertilizationEditDto.CropId}");
            }

            var fertilization = new Fertilization
            {
                Id = Guid.NewGuid(),
                Crop = crop,
                Date = fertilizationEditDto.Date,
                Type = fertilizationEditDto.Type,
                Quantity = fertilizationEditDto.Quantity,
                Method = fertilizationEditDto.Method,
                Description = fertilizationEditDto.Description
            };

            await _fertilizationRepository.Add(fertilization);

            return new FertilizationDTO
            {
                Id = fertilization.Id,
                Crop = new MiniItemDTO { Id = fertilization.Crop.Id.ToString(), Name = fertilization.Crop.Name },
                Date = fertilization.Date,
                Type = fertilization.Type,
                Quantity = fertilization.Quantity,
                Method = fertilization.Method,
                Description = fertilization.Description
            };
        }

        public async Task DeleteFertilization(Guid id)
        {
            await _fertilizationRepository.Delete(id);
        }

        public async Task<FertilizationDTO> GetFertilizationById(Guid id)
        {
            var fertilization = await _fertilizationRepository.GetById(id);
            if (fertilization == null)
            {
                throw new Exception($"Fertilization not found with ID: {id}");
            }

            return new FertilizationDTO
            {
                Id = fertilization.Id,
                Crop = new MiniItemDTO { Id = fertilization.Crop.Id.ToString(), Name = fertilization.Crop.Name },
                Date = fertilization.Date,
                Type = fertilization.Type,
                Quantity = fertilization.Quantity,
                Method = fertilization.Method,
                Description = fertilization.Description
            };
        }

        public async Task<IEnumerable<FertilizationDTO>> GetFertilizationsByCropId(Guid cropId)
        {
            var fertilizations = await _fertilizationRepository.GetFertilizationsByCropId(cropId);
            return fertilizations.Select(fertilization => new FertilizationDTO
            {
                Id = fertilization.Id,
                Crop = new MiniItemDTO { Id = fertilization.Crop.Id.ToString(), Name = fertilization.Crop.Name },
                Date = fertilization.Date,
                Type = fertilization.Type,
                Quantity = fertilization.Quantity,
                Method = fertilization.Method,
                Description = fertilization.Description
            }).ToList();
        }

        public async Task<FertilizationDTO> UpdateFertilization(Guid id, FertilizationEditDTO fertilizationEditDto)
        {
            var fertilization = await _fertilizationRepository.GetById(id);
            if (fertilization == null)
            {
                throw new Exception($"Fertilization not found with ID: {id}");
            }

            var crop = await _cropRepository.GetById(fertilizationEditDto.CropId);
            if (crop == null)
            {
                throw new Exception("Crop not found");
            }

            fertilization.Crop = crop;
            fertilization.Date = fertilizationEditDto.Date;
            fertilization.Type = fertilizationEditDto.Type;
            fertilization.Quantity = fertilizationEditDto.Quantity;
            fertilization.Method = fertilizationEditDto.Method;
            fertilization.Description = fertilizationEditDto.Description;

            await _fertilizationRepository.Update(fertilization);

            return new FertilizationDTO
            {
                Id = fertilization.Id,
                Crop = new MiniItemDTO { Id = fertilization.Crop.Id.ToString(), Name = fertilization.Crop.Name },
                Date = fertilization.Date,
                Type = fertilization.Type,
                Quantity = fertilization.Quantity,
                Method = fertilization.Method,
                Description = fertilization.Description
            };
        }
    }
}
