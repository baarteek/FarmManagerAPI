using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;

namespace FarmManagerAPI.Services.Implementations;

public class CultivationOperationService : ICultivationOperationService
{
    private readonly ICultivationOperationRepository _cultivationOperationRepository;
    private readonly ICropRepository _cropRepository;

    public CultivationOperationService(ICultivationOperationRepository cultivationOperationRepository, ICropRepository cropRepository)
    {
        _cultivationOperationRepository = cultivationOperationRepository;
        _cropRepository = cropRepository;
    }
    
    public async Task<CultivationOperationDTO> GetCultivationOperationById(Guid id)
    {
        var cultivationOperation = await _cultivationOperationRepository.GetById(id);
        if (cultivationOperation == null)
        {
            throw new Exception($"Could not find cultivation operation with id: {id}");
        }

        return new CultivationOperationDTO
        {
            Id = cultivationOperation.Id,
            Name = cultivationOperation.Name,
            Date = cultivationOperation.Date,
            AgrotechnicalIntervention = cultivationOperation.AgrotechnicalIntervention != null ? Enum.GetName(typeof(AgrotechnicalIntervention), cultivationOperation.AgrotechnicalIntervention) : null,
            Description = cultivationOperation.Description,
            Crop = new MiniItemDTO { Id = cultivationOperation.Crop.Id.ToString(), Name = cultivationOperation.Crop.Name}
        };
    }

    public async Task<IEnumerable<CultivationOperationDTO>> GetCultivationOperationsByCropId(Guid cropId)
    {
        var cultivationOperations = await _cultivationOperationRepository.GetCultivationOperationsByCropId(cropId);

        return cultivationOperations.Select(operation => new CultivationOperationDTO
        {
            Id = operation.Id,
            Name = operation.Name,
            Date = operation.Date,
            AgrotechnicalIntervention = operation.AgrotechnicalIntervention != null ? Enum.GetName(typeof(AgrotechnicalIntervention), operation.AgrotechnicalIntervention) : null,
            Description = operation.Description,
            Crop = new MiniItemDTO { Id = operation.Crop.Id.ToString(), Name = operation.Crop.Name}
        });
    }

    public async Task AddCultivationOperation(CultivationOperationEditDTO operation)
    {
        var crop = await _cropRepository.GetById(operation.CropId);
        if (crop == null)
        {
            throw new Exception($"Crop not found with ID: {operation.CropId}");
        }
        
        var cultivationOperation = new CultivationOperation
        {
            Id = Guid.NewGuid(),
            Name = operation.Name,
            Date = operation.Date,
            AgrotechnicalIntervention = operation.AgrotechnicalIntervention,
            Description = operation.Description,
            Crop = crop
        };
        
        await _cultivationOperationRepository.Add(cultivationOperation);
    }

    public async Task UpdateCultivationOperation(Guid id, CultivationOperationEditDTO operation)
    {
        var cultivationOperation = await _cultivationOperationRepository.GetById(id);
        if (cultivationOperation == null)
        {
            throw new Exception($"Cultivation operation with ID: {id} not found");
        }
        
        var crop = await _cropRepository.GetById(operation.CropId);
        if (crop == null)
        {
            throw new Exception($"Crop not found with ID: {operation.CropId}");
        }
        
        cultivationOperation.Name = operation.Name;
        cultivationOperation.Date = operation.Date;
        cultivationOperation.AgrotechnicalIntervention = operation.AgrotechnicalIntervention;
        cultivationOperation.Description = operation.Description;
        cultivationOperation.Crop = crop;
        
        await _cultivationOperationRepository.Update(cultivationOperation);
    }

    public async Task DeleteCultivationOperation(Guid id)
    {
        await _cultivationOperationRepository.Delete(id);
    }
}