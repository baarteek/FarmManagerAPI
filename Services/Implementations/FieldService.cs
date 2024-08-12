using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmManagerAPI.Services.Implementations
{
    public class FieldService : IFieldService
    {
        private readonly IFieldRepository _fieldRepository;
        private readonly IFarmRepository _farmRepository;
        private readonly IReferenceParcelRepository _referenceParcelRepository;
        private readonly ISoilMeasurementRepository _soilMeasurementRepository;
        private readonly ICropRepository _cropRepository;

        public FieldService(IFieldRepository fieldRepository, IFarmRepository farmRepository, IReferenceParcelRepository referenceParcelRepository, ISoilMeasurementRepository soilMeasurementRepository, ICropRepository cropRepository)
        {
            _fieldRepository = fieldRepository;
            _farmRepository = farmRepository;
            _referenceParcelRepository = referenceParcelRepository;
            _soilMeasurementRepository = soilMeasurementRepository;
            _cropRepository = cropRepository;
        }

        public async Task<FieldDTO> AddField(FieldEditDTO fieldEditDto)
        {
            var farm = await _farmRepository.GetById(fieldEditDto.FarmId);
            if (farm == null)
            {
                throw new Exception("Farm not found");
            }

            var field = new Field
            {
                Id = Guid.NewGuid(),
                Name = fieldEditDto.Name,
                Area = fieldEditDto.Area,
                SoilType = fieldEditDto.SoilType,
                Farm = farm
            };

            await _fieldRepository.Add(field);

            return new FieldDTO
            {
                Id = field.Id,
                Farm = new MiniItemDTO { Id = field.Farm.Id.ToString(), Name = field.Farm.Name },
                Name = field.Name,
                Area = field.Area,
                SoilType = field.SoilType,
                ReferenceParcels = field.ReferenceParcels?.Select(rp => new MiniItemDTO { Id = rp.Id.ToString(), Name = rp.ParcelNumber }).ToList(),
                SoilMeasurements = field.SoilMeasurements?.Select(sm => new MiniItemDTO { Id = sm.Id.ToString(), Name = sm.Date.ToString("yyyy-MM-dd") }).ToList(),
                Crops = field.Crops?.Select(c => new MiniItemDTO { Id = c.Id.ToString(), Name = c.Name }).ToList()
            };
        }

        public async Task DeleteField(Guid id)
        {
            var field = await _fieldRepository.GetById(id);
            if (field == null)
            {
                throw new Exception("Field not found");
            }

            await _fieldRepository.Delete(id);
        }

        public async Task<FieldDTO> GetFieldById(Guid id)
        {
            var field = await _fieldRepository.GetById(id);
            if (field == null)
            {
                throw new Exception("Field not found");
            }

            if (field.Farm == null)
            {
                throw new Exception("Farm reference is missing for the field");
            }

            field.ReferenceParcels = (await _referenceParcelRepository.GetParcelsByFieldId(field.Id)).ToList();
            field.SoilMeasurements = (await _soilMeasurementRepository.GetSoilMeasurementsByFieldId(field.Id)).ToList();
            field.Crops = (await _cropRepository.GetCropsByFieldId(field.Id)).ToList();

            return new FieldDTO
            {
                Id = field.Id,
                Farm = new MiniItemDTO { Id = field.Farm.Id.ToString(), Name = field.Farm.Name },
                Name = field.Name,
                Area = field.Area,
                SoilType = field.SoilType,
                ReferenceParcels = field.ReferenceParcels?.Select(rp => new MiniItemDTO { Id = rp.Id.ToString(), Name = rp.ParcelNumber }).ToList(),
                SoilMeasurements = field.SoilMeasurements?.Select(sm => new MiniItemDTO { Id = sm.Id.ToString(), Name = sm.Date.ToString("yyyy-MM-dd") }).ToList(),
                Crops = field.Crops?.Select(c => new MiniItemDTO { Id = c.Id.ToString(), Name = c.Name }).ToList()
            };
        }

        public async Task<IEnumerable<FieldDTO>> GetFieldsByFarmId(Guid farmId)
        {
            var fields = await _fieldRepository.GetFieldsByFarmId(farmId);

            foreach(var field in fields)
            {
                field.ReferenceParcels = (await _referenceParcelRepository.GetParcelsByFieldId(field.Id)).ToList();
                field.SoilMeasurements = (await _soilMeasurementRepository.GetSoilMeasurementsByFieldId(field.Id)).ToList();
                field.Crops = (await _cropRepository.GetCropsByFieldId(field.Id)).ToList();

            }

            return fields.Select(field => new FieldDTO
            {
                Id = field.Id,
                Farm = new MiniItemDTO { Id = field.Farm.Id.ToString(), Name = field.Farm.Name },
                Name = field.Name,
                Area = field.Area,
                SoilType = field.SoilType,
                ReferenceParcels = field.ReferenceParcels?.Select(rp => new MiniItemDTO { Id = rp.Id.ToString(), Name = rp.ParcelNumber }).ToList(),
                SoilMeasurements = field.SoilMeasurements?.Select(sm => new MiniItemDTO { Id = sm.Id.ToString(), Name = sm.Date.ToString("yyyy-MM-dd") }).ToList(),
                Crops = field.Crops?.Select(c => new MiniItemDTO { Id = c.Id.ToString(), Name = c.Name }).ToList()
            }).ToList();
        }

        public async Task<FieldDTO> UpdateField(Guid id, FieldEditDTO fieldEditDto)
        {
            var field = await _fieldRepository.GetById(id);
            if (field == null)
            {
                throw new Exception("Field not found");
            }

            var farm = await _farmRepository.GetById(fieldEditDto.FarmId);
            if (farm == null)
            {
                throw new Exception("Farm not found");
            }

            field.Name = fieldEditDto.Name;
            field.Area = fieldEditDto.Area;
            field.SoilType = fieldEditDto.SoilType;
            field.Farm = farm;

            await _fieldRepository.Update(field);

            return new FieldDTO
            {
                Id = field.Id,
                Farm = new MiniItemDTO { Id = field.Farm.Id.ToString(), Name = field.Farm.Name },
                Name = field.Name,
                Area = field.Area,
                SoilType = field.SoilType,
                ReferenceParcels = field.ReferenceParcels?.Select(rp => new MiniItemDTO { Id = rp.Id.ToString(), Name = rp.ParcelNumber }).ToList(),
                SoilMeasurements = field.SoilMeasurements?.Select(sm => new MiniItemDTO { Id = sm.Id.ToString(), Name = sm.Date.ToString("yyyy-MM-dd") }).ToList(),
                Crops = field.Crops?.Select(c => new MiniItemDTO { Id = c.Id.ToString(), Name = c.Name }).ToList()
            };
        }
    }
}
