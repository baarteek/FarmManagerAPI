﻿using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;

namespace FarmManagerAPI.Services.Implementations
{
    public class PlantProtectionService : IPlantProtectionService
    {
        private readonly IPlantProtectionRepository _plantProtectionRepository;
        private readonly ICropRepository _cropRepository;

        public PlantProtectionService(IPlantProtectionRepository plantProtectionRepository, ICropRepository cropRepository)
        {
            _plantProtectionRepository = plantProtectionRepository;
            _cropRepository = cropRepository;
        }

        public async Task<PlantProtectionDTO> AddPlantProtection(PlantProtectionEditDTO plantProtectionEditDto)
        {
            var crop = await _cropRepository.GetById(plantProtectionEditDto.CropId);
            if (crop == null)
            {
                throw new Exception("Crop not found");
            }

            var plantProtection = new PlantProtection
            {
                Id = Guid.NewGuid(),
                Crop = crop,
                Date = plantProtectionEditDto.Date,
                Type = plantProtectionEditDto.Type,
                Quantity = plantProtectionEditDto.Quantity,
                NameOfProduct = plantProtectionEditDto.NameOfProduct,
                Method = plantProtectionEditDto.Method,
                Description = plantProtectionEditDto.Description
            };

            await _plantProtectionRepository.Add(plantProtection);

            return new PlantProtectionDTO
            {
                Id = plantProtection.Id,
                Crop = new MiniItemDTO { Id = plantProtection.Crop.Id.ToString(), Name = plantProtection.Crop.Name },
                Date = plantProtection.Date,
                Type = plantProtection.Type,
                Quantity = plantProtection.Quantity,
                NameOfProduct = plantProtection.NameOfProduct,
                Method = plantProtection.Method,
                Description = plantProtection.Description
            };
        }

        public async Task DeletePlantProtection(Guid id)
        {
            await _plantProtectionRepository.Delete(id);
        }

        public async Task<PlantProtectionDTO> GetPlantProtectionById(Guid id)
        {
            var plantProtection = await _plantProtectionRepository.GetById(id);
            if (plantProtection == null)
            {
                throw new Exception("PlantProtection not found");
            }

            return new PlantProtectionDTO
            {
                Id = plantProtection.Id,
                Crop = new MiniItemDTO { Id = plantProtection.Crop.Id.ToString(), Name = plantProtection.Crop.Name },
                Date = plantProtection.Date,
                Type = plantProtection.Type,
                NameOfProduct = plantProtection.NameOfProduct,
                Quantity = plantProtection.Quantity,
                Method = plantProtection.Method,
                Description = plantProtection.Description
            };
        }

        public async Task<IEnumerable<PlantProtectionDTO>> GetPlantProtectionsByCropId(Guid cropId)
        {
            var plantProtections = await _plantProtectionRepository.GetPlantProtectionsByCropId(cropId);
            return plantProtections.Select(pp => new PlantProtectionDTO
            {
                Id = pp.Id,
                Crop = new MiniItemDTO { Id = pp.Crop.Id.ToString(), Name = pp.Crop.Name },
                Date = pp.Date,
                Type = pp.Type,
                NameOfProduct = pp.NameOfProduct,
                Quantity = pp.Quantity,
                Method = pp.Method,
                Description = pp.Description
            }).ToList();
        }

        public async Task<PlantProtectionDTO> UpdatePlantProtection(Guid id, PlantProtectionEditDTO plantProtectionEditDto)
        {
            var plantProtection = await _plantProtectionRepository.GetById(id);
            if (plantProtection == null)
            {
                throw new Exception("PlantProtection not found");
            }

            var crop = await _cropRepository.GetById(plantProtectionEditDto.CropId);
            if (crop == null)
            {
                throw new Exception("Crop not found");
            }

            plantProtection.Crop = crop;
            plantProtection.Date = plantProtectionEditDto.Date;
            plantProtection.Type = plantProtectionEditDto.Type;
            plantProtection.NameOfProduct = plantProtectionEditDto.NameOfProduct;
            plantProtection.Quantity = plantProtectionEditDto.Quantity;
            plantProtection.Method = plantProtectionEditDto.Method;
            plantProtection.Description = plantProtectionEditDto.Description;

            await _plantProtectionRepository.Update(plantProtection);

            return new PlantProtectionDTO
            {
                Id = plantProtection.Id,
                Crop = new MiniItemDTO { Id = plantProtection.Crop.Id.ToString(), Name = plantProtection.Crop.Name },
                Date = plantProtection.Date,
                Type = plantProtection.Type,
                NameOfProduct = plantProtection.NameOfProduct,
                Quantity = plantProtection.Quantity,
                Method = plantProtection.Method,
                Description = plantProtection.Description
            };
        }
    }
}
