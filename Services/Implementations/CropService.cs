using System.Globalization;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FarmManagerAPI.Services.Implementations
{
    public class CropService : ICropService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ICropRepository _cropRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly IFertilizationRepository _fertilizationRepository;
        private readonly IPlantProtectionRepository _plantProtectionRepository;
        private readonly ICultivationOperationRepository _cultivationOperationRepository;

        public CropService(ICropRepository cropRepository, IFieldRepository fieldRepository, IFertilizationRepository fertilizationRepository, IPlantProtectionRepository plantProtectionRepository, ICultivationOperationRepository cultivationOperationRepository , UserManager<IdentityUser> userManager)
        {
            _cropRepository = cropRepository;
            _userManager = userManager;
            _fieldRepository = fieldRepository;
            _fertilizationRepository = fertilizationRepository;
            _plantProtectionRepository = plantProtectionRepository;
            _cultivationOperationRepository = cultivationOperationRepository;
        }

        public async Task<CropDTO> AddCrop(CropEditDTO cropEditDTO)
        {
            var field = await _fieldRepository.GetById(cropEditDTO.FieldId);
            if (field == null)
            {
                throw new Exception($"Field not found with ID: {cropEditDTO.FieldId}");
            }
            
            if (cropEditDTO.IsActive)
            {
                var activeCrops = await _cropRepository.GetCropsByFieldId(field.Id);
                foreach (var activeCrop in activeCrops)
                {
                    if (activeCrop.IsActive)
                    {
                        activeCrop.IsActive = false;
                        await _cropRepository.Update(activeCrop);
                    }
                }
            }
            
            var crop = new Crop
            {
                Id = Guid.NewGuid(),
                Field = field,
                Name = cropEditDTO.Name,
                Type = cropEditDTO.Type,
                IsActive = cropEditDTO.IsActive,
            };

            await _cropRepository.Add(crop);

            return new CropDTO
            {
                Id = crop.Id,
                Field = new MiniItemDTO { Id = crop.Field.Id.ToString(), Name = crop.Field.Name },
                Name = crop.Name,
                Type = crop.Type,
                IsActive = crop.IsActive,
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
            crop.CultivationOperations = (await _cultivationOperationRepository.GetCultivationOperationsByCropId(id)).ToList();
            

            return new CropDTO
            {
                Id = crop.Id,
                Field = new MiniItemDTO { Id = crop.Field.Id.ToString(), Name = crop.Field.Name },
                Name = crop.Name,
                Type = crop.Type,
                IsActive = crop.IsActive,
                CultivationOperations = crop.CultivationOperations?.Select(co => new MiniItemDTO { Id = co.Id.ToString(), Name = co.Name }).ToList(),
                Fertilizations = crop.Fertilizations?.Select(f => new MiniItemDTO { Id = f.Id.ToString(), Name = f.Date.ToString(CultureInfo.InvariantCulture) }).ToList(),
                PlantProtections = crop.PlantProtections?.Select(pp => new MiniItemDTO { Id = pp.Id.ToString(), Name = pp.Date.ToString(CultureInfo.InvariantCulture) }).ToList()
            };
        }

        public async Task<IEnumerable<CropDTO>> GetCropsByFieldId(Guid fieldId)
        {
            var crops = await _cropRepository.GetCropsByFieldId(fieldId);

            foreach (var crop in crops)
            {
                crop.Fertilizations = (await _fertilizationRepository.GetFertilizationsByCropId(crop.Id)).ToList();
                crop.PlantProtections = (await _plantProtectionRepository.GetPlantProtectionsByCropId(crop.Id)).ToList();
                crop.CultivationOperations = (await _cultivationOperationRepository.GetCultivationOperationsByCropId(crop.Id)).ToList();
            }

            return crops.Select(crop => new CropDTO
            {
                Id = crop.Id,
                Field = new MiniItemDTO { Id = crop.Field.Id.ToString(), Name = crop.Field.Name },
                Name = crop.Name,
                Type = crop.Type,
                IsActive = crop.IsActive,
                CultivationOperations = crop.CultivationOperations?.Select(co => new MiniItemDTO { Id = co.Id.ToString(), Name = co.Name }).ToList(),
                Fertilizations = crop.Fertilizations?.Select(f => new MiniItemDTO { Id = f.Id.ToString(), Name = f.Date.ToString() }).ToList(),
                PlantProtections = crop.PlantProtections?.Select(pp => new MiniItemDTO { Id = pp.Id.ToString(), Name = pp.Date.ToString() }).ToList()
            }).ToList();
        }

        public async Task<IEnumerable<CropDTO>> GetActiveCropsByUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if(user == null)
            {
                throw new Exception($"User not found with username: {userName}");
            }

            var crops = await _cropRepository.GetActiveCropsByUserId(user.Id);

            foreach (var crop in crops)
            {
                crop.Fertilizations = (await _fertilizationRepository.GetFertilizationsByCropId(crop.Id)).ToList();
                crop.PlantProtections = (await _plantProtectionRepository.GetPlantProtectionsByCropId(crop.Id)).ToList();
                crop.CultivationOperations = (await _cultivationOperationRepository.GetCultivationOperationsByCropId(crop.Id)).ToList();
            }

            return crops.Select(crop => new CropDTO
            {
                Id = crop.Id,
                Field = new MiniItemDTO { Id = crop.Field.Id.ToString(), Name = crop.Field.Name },
                Name = crop.Name,
                Type = crop.Type,
                IsActive = crop.IsActive,
                CultivationOperations = crop.CultivationOperations?.Select(co => new MiniItemDTO { Id = co.Id.ToString(), Name = co.Name }).ToList(),
                Fertilizations = crop.Fertilizations?.Select(f => new MiniItemDTO { Id = f.Id.ToString(), Name = f.Date.ToString() }).ToList(),
                PlantProtections = crop.PlantProtections?.Select(pp => new MiniItemDTO { Id = pp.Id.ToString(), Name = pp.Date.ToString() }).ToList()
            });
        }

        public async Task<IEnumerable<CropDTO>> GetCropsByUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if(user == null)
            {
                throw new Exception($"User not found with username: {userName}");
            }

            var crops = await _cropRepository.GetCropsByUserId(user.Id);
            
            foreach(var crop in crops)
            {
                crop.Fertilizations = (await _fertilizationRepository.GetFertilizationsByCropId(crop.Id)).ToList();
                crop.PlantProtections = (await _plantProtectionRepository.GetPlantProtectionsByCropId(crop.Id)).ToList();
                crop.CultivationOperations = (await _cultivationOperationRepository.GetCultivationOperationsByCropId(crop.Id)).ToList();
            }

            return crops.Select(crop => new CropDTO
            {
                Id = crop.Id,
                Field = new MiniItemDTO { Id = crop.Field.Id.ToString(), Name = crop.Field.Name },
                Name = crop.Name,
                Type = crop.Type,
                IsActive = crop.IsActive,
                CultivationOperations = crop.CultivationOperations?.Select(co => new MiniItemDTO { Id = co.Id.ToString(), Name = co.Name }).ToList(),
                Fertilizations = crop.Fertilizations?.Select(f => new MiniItemDTO { Id = f.Id.ToString(), Name = f.Date.ToString() }).ToList(),
                PlantProtections = crop.PlantProtections?.Select(pp => new MiniItemDTO { Id = pp.Id.ToString(), Name = pp.Date.ToString() }).ToList()
            });
        }

        public async Task UpdateCrop(Guid id, CropEditDTO cropEditDTO)
        {
            var crop = await _cropRepository.GetById(id);
            if (crop == null)
            {
                throw new Exception($"Crop not found with ID: {id}");
            }
            
            if (cropEditDTO.IsActive && !crop.IsActive)
            {
                var activeCrops = await _cropRepository.GetCropsByFieldId(cropEditDTO.FieldId);
                foreach (var activeCrop in activeCrops)
                {
                    if (activeCrop.IsActive)
                    {
                        activeCrop.IsActive = false;
                        await _cropRepository.Update(activeCrop);
                    }
                }
            }
            
            crop.Name = cropEditDTO.Name;
            crop.Type = cropEditDTO.Type;
            crop.IsActive = cropEditDTO.IsActive;

            await _cropRepository.Update(crop);
        }

        public async Task<IEnumerable<EnumDTO>> GetAgrotechnicalInterventions()
        {
            return await Task.Run(() =>
            {
                return Enum.GetValues(typeof(AgrotechnicalIntervention))
                .Cast<AgrotechnicalIntervention>()
                .Select(intervention => new EnumDTO
                {
                    Value = (int)intervention,
                    Name = intervention.ToString(),
                    Description = intervention.GetDescription(),
                });
            });
        }
    }
}
