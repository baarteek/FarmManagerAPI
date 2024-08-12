using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;

namespace FarmManagerAPI.Services.Implementations
{
    public class CropService : ICropService
    {
        private readonly ICropRepository _cropRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly IFertilizationRepository _fertilizationRepository;
        private readonly IPlantProtectionRepository _plantProtectionRepository;

        public CropService(ICropRepository cropRepository, IFieldRepository fieldRepository, IFertilizationRepository fertilizationRepository, IPlantProtectionRepository plantProtectionRepository)
        {
            _cropRepository = cropRepository;
            _fieldRepository = fieldRepository;
            _fertilizationRepository = fertilizationRepository;
            _plantProtectionRepository = plantProtectionRepository;
        }

        public async Task<CropDTO> AddCrop(CropEditDTO cropEditDTO)
        {
            var field = await _fieldRepository.GetById(cropEditDTO.FieldId);
            if(field == null)
            {
                throw new Exception($"Field not found with ID: {cropEditDTO.FieldId}");
            }

            var crop = new Crop
            {
                Id = Guid.NewGuid(),
                Field = field,
                Name = cropEditDTO.Name,
                Type = cropEditDTO.Type,
                SowingDate = cropEditDTO.SowingDate,
                HarvestDate = cropEditDTO.HarvestDate,
                IsActive = cropEditDTO.IsActive,
            };

            await _cropRepository.Add(crop);

            return new CropDTO
            {
                Id = crop.Id,
                Field = new MiniItemDTO { Id = crop.Field.Id.ToString(), Name = crop.Field.Name },
                Name = crop.Name,
                Type = crop.Type,
                SowingDate = crop.SowingDate,
                HarvestDate = crop.HarvestDate,
                IsActive= crop.IsActive,
            };
        }

        public async Task DeleteCrop(Guid id)
        {
            await _cropRepository.Delete(id);
        }

        public async Task<CropDTO> GetCropById(Guid id)
        {
            var crop = await _cropRepository.GetById(id);
            if (crop == null)
            {
                throw new Exception($"Crop not found with ID: {crop.Field.Id}");
            }

            crop.Fertilizations = (await _fertilizationRepository.GetFertilizationsByCropId(id)).ToList();
            crop.PlantProtections = (await _plantProtectionRepository.GetPlantProtectionsByCropId(id)).ToList();

            return new CropDTO
            {
                Id = crop.Id,
                Field = new MiniItemDTO { Id = crop.Field.Id.ToString(), Name = crop.Field.Name },
                Name = crop.Name,
                Type = crop.Type,
                SowingDate = crop.SowingDate,
                HarvestDate = crop.HarvestDate,
                IsActive = crop.IsActive,
                Fertilizations = crop.Fertilizations?.Select(f => new MiniItemDTO { Id = f.Id.ToString(), Name = f.Type.ToString() }).ToList(),
                PlantProtections = crop.PlantProtections?.Select(pp => new MiniItemDTO { Id = pp.Id.ToString(), Name = pp.Type.ToString() }).ToList()
            };
        }

        public async Task<IEnumerable<CropDTO>> GetCropsByFieldId(Guid fieldId)
        {
            var crops = await _cropRepository.GetCropsByFieldId(fieldId);

            foreach (var crop in crops)
            {
                crop.Fertilizations = (await _fertilizationRepository.GetFertilizationsByCropId(crop.Id)).ToList();
                crop.PlantProtections = (await _plantProtectionRepository.GetPlantProtectionsByCropId(crop.Id)).ToList();
            }

            return crops.Select(crop => new CropDTO
            {
                Id = crop.Id,
                Field = new MiniItemDTO { Id = crop.Field.Id.ToString(), Name = crop.Field.Name },
                Name = crop.Name,
                Type = crop.Type,
                SowingDate = crop.SowingDate,
                HarvestDate = crop.HarvestDate,
                IsActive = crop.IsActive,
                Fertilizations = crop.Fertilizations?.Select(f => new MiniItemDTO { Id = f.Id.ToString(), Name = f.Type.ToString() }).ToList(),
                PlantProtections = crop.PlantProtections?.Select(pp => new MiniItemDTO { Id = pp.Id.ToString(), Name = pp.Type.ToString() }).ToList()
            }).ToList();
        }

        public async Task UdpateCrop(Guid id, CropEditDTO cropEditDTO)
        {
            var crop = await _cropRepository.GetById(id);
            if (crop == null)
            {
                throw new Exception($"Crop not found with ID: {crop.Field.Id}");
            }

            crop.Name = cropEditDTO.Name;
            crop.Type = cropEditDTO.Type;
            crop.SowingDate = cropEditDTO.SowingDate;
            crop.HarvestDate = cropEditDTO.HarvestDate;
            crop.IsActive = cropEditDTO.IsActive;

            await _cropRepository.Update(crop);
        }
    }
}
