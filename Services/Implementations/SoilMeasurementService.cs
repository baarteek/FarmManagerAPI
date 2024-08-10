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
    public class SoilMeasurementService : ISoilMeasurementService
    {
        private readonly ISoilMeasurementRepository _soilMeasurementRepository;
        private readonly IFieldRepository _fieldRepository;

        public SoilMeasurementService(ISoilMeasurementRepository soilMeasurementRepository, IFieldRepository fieldRepository)
        {
            _soilMeasurementRepository = soilMeasurementRepository;
            _fieldRepository = fieldRepository;
        }

        public async Task<SoilMeasurementDTO> AddSoilMeasurement(SoilMeasurementEditDTO soilMeasurementEditDto)
        {
            var field = await _fieldRepository.GetById(soilMeasurementEditDto.FieldId);
            if (field == null)
            {
                throw new Exception("Field not found");
            }

            var soilMeasurement = new SoilMeasurement
            {
                Id = Guid.NewGuid(),
                Field = field,
                Date = soilMeasurementEditDto.Date,
                pH = soilMeasurementEditDto.pH,
                Nitrogen = soilMeasurementEditDto.Nitrogen,
                Phosphorus = soilMeasurementEditDto.Phosphorus,
                Potassium = soilMeasurementEditDto.Potassium
            };

            await _soilMeasurementRepository.Add(soilMeasurement);

            return new SoilMeasurementDTO
            {
                Id = soilMeasurement.Id,
                Field = new MiniItemDTO { Id = field.Id.ToString(), Name = field.Name },
                Date = soilMeasurement.Date,
                pH = soilMeasurement.pH,
                Nitrogen = soilMeasurement.Nitrogen,
                Phosphorus = soilMeasurement.Phosphorus,
                Potassium = soilMeasurement.Potassium
            };
        }

        public async Task DeleteSoilMeasurement(Guid id)
        {
            await _soilMeasurementRepository.Delete(id);
        }

        public async Task<SoilMeasurementDTO> GetSoilMeasurementById(Guid id)
        {
            var soilMeasurement = await _soilMeasurementRepository.GetById(id);
            if (soilMeasurement == null)
            {
                throw new Exception($"Soil Measurement not found with ID: {id}");
            }

            return new SoilMeasurementDTO
            {
                Id = soilMeasurement.Id,
                Field = new MiniItemDTO { Id = soilMeasurement.Field.Id.ToString(), Name = soilMeasurement.Field.Name },
                Date = soilMeasurement.Date,
                pH = soilMeasurement.pH,
                Nitrogen = soilMeasurement.Nitrogen,
                Phosphorus = soilMeasurement.Phosphorus,
                Potassium = soilMeasurement.Potassium
            };
        }

        public async Task<IEnumerable<SoilMeasurementDTO>> GetSoilMeasurementsByFieldId(Guid fieldId)
        {
            var soilMeasurements = await _soilMeasurementRepository.GetSoilMeasurementsByFieldId(fieldId);
            return soilMeasurements.Select(sm => new SoilMeasurementDTO
            {
                Id = sm.Id,
                Field = new MiniItemDTO { Id = sm.Field.Id.ToString(), Name = sm.Field.Name },
                Date = sm.Date,
                pH = sm.pH,
                Nitrogen = sm.Nitrogen,
                Phosphorus = sm.Phosphorus,
                Potassium = sm.Potassium
            }).ToList();
        }

        public async Task<SoilMeasurementDTO> UpdateSoilMeasurement(Guid id, SoilMeasurementEditDTO soilMeasurementEditDto)
        {
            var soilMeasurement = await _soilMeasurementRepository.GetById(id);
            var field = await _fieldRepository.GetById(soilMeasurementEditDto.FieldId);

            if (soilMeasurement == null || field == null)
            {
                throw new Exception("Soil Measurement or Field not found");
            }

            soilMeasurement.Field = field;
            soilMeasurement.Date = soilMeasurementEditDto.Date;
            soilMeasurement.pH = soilMeasurementEditDto.pH;
            soilMeasurement.Nitrogen = soilMeasurementEditDto.Nitrogen;
            soilMeasurement.Phosphorus = soilMeasurementEditDto.Phosphorus;
            soilMeasurement.Potassium = soilMeasurementEditDto.Potassium;

            await _soilMeasurementRepository.Update(soilMeasurement);

            return new SoilMeasurementDTO
            {
                Id = soilMeasurement.Id,
                Field = new MiniItemDTO { Id = field.Id.ToString(), Name = field.Name },
                Date = soilMeasurement.Date,
                pH = soilMeasurement.pH,
                Nitrogen = soilMeasurement.Nitrogen,
                Phosphorus = soilMeasurement.Phosphorus,
                Potassium = soilMeasurement.Potassium
            };
        }
    }
}
