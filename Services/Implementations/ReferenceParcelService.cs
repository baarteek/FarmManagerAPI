using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;

namespace FarmManagerAPI.Services.Implementations
{
    public class ReferenceParcelService : IReferenceParcelService
    {
        private readonly IReferenceParcelRepository _parcelRepository;
        private readonly IFieldRepository _fieldRepository;

        public ReferenceParcelService(IReferenceParcelRepository parcelRepository, IFieldRepository fieldRepository)
        {
            _parcelRepository = parcelRepository;
            _fieldRepository = fieldRepository;
        }

        public async Task<ReferenceParcelDTO> AddParcel(ReferenceParcelEditDTO parcelEditDto)
        {
            var field = await _fieldRepository.GetById(parcelEditDto.FieldId);
            if (field == null)
            {
                throw new Exception($"Field not found with ID: {parcelEditDto.FieldId}");
            }

            var parcel = new ReferenceParcel
            {
                Id = Guid.NewGuid(),
                Field = field,
                ParcelNumber = parcelEditDto.ParcelNumber,
                Area = parcelEditDto.Area
            };

            await _parcelRepository.Add(parcel);

            return new ReferenceParcelDTO
            {
                Id = parcel.Id,
                Field = new MiniItemDTO { Id = parcel.Field.Id.ToString(), Name = parcel.Field.Name },
                ParcelNumber = parcel.ParcelNumber,
                Area = parcel.Area
            };
        }

        public async Task<ReferenceParcelDTO> GetParcelById(Guid id)
        {
            var parcel = await _parcelRepository.GetById(id);
            if (parcel == null)
            {
                throw new Exception($"Parcel not found with ID: {parcel.Id}");
            }

            return new ReferenceParcelDTO
            {
                Id = parcel.Id,
                Field = new MiniItemDTO { Id = parcel.Field.Id.ToString(), Name = parcel.Field.Name },
                ParcelNumber = parcel.ParcelNumber,
                Area = parcel.Area
            };
        }

        public async Task<IEnumerable<ReferenceParcelDTO>> GetParcelsByFieldId(Guid fieldId)
        {
            var parcels = await _parcelRepository.GetParcelsByFieldId(fieldId);
            return parcels.Select(parcel => new ReferenceParcelDTO
            {
                Id = parcel.Id,
                Field = new MiniItemDTO { Id = parcel.Field.Id.ToString(), Name = parcel.Field.Name },
                ParcelNumber = parcel.ParcelNumber,
                Area = parcel.Area
            }).ToList();
        }

        public async Task UpdateParcel(Guid id, ReferenceParcelEditDTO parcelEditDto)
        {
            var parcel = await _parcelRepository.GetById(id);
            if (parcel == null)
            {
                throw new Exception($"Parcel not found with ID: {parcel.Id}");
            }

            var field = await _fieldRepository.GetById(parcelEditDto.FieldId);
            if (field == null)
            {
                throw new Exception($"Field not found with ID: {field.Id}");
            }

            parcel.ParcelNumber = parcelEditDto.ParcelNumber;
            parcel.Area = parcelEditDto.Area;
            parcel.Field = field;

            await _parcelRepository.Update(parcel);
        }

        public async Task DeleteParcel(Guid id)
        {
            await _parcelRepository.Delete(id);
        }
    }
}
