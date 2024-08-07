﻿using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;

namespace FarmManagerAPI.Services.Interfaces
{
    public interface ICropService
    {
        Task<CropDTO> GetCropById(Guid id);
        Task<IEnumerable<CropDTO>> GetCropsByFieldId(Guid fieldId);
        Task<Crop> AddCrop(CropEditDTO cropEditDTO);
        Task UdpateCrop(CropEditDTO cropEditDTO);
        Task DeleteCrop(Guid id);
    }
}
