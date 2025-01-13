using FarmManagerAPI.DTOs;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FarmManagerAPI.Services.Implementations;

public class HomePageService : IHomePageService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ICropRepository _cropRepository;
    private readonly IFertilizationRepository _fertilizationRepository;
    private readonly IPlantProtectionRepository _plantProtectionRepository;
    private readonly ICultivationOperationRepository _cultivationOperationRepository;

    public HomePageService(
        UserManager<IdentityUser> userManager,
        IFertilizationRepository fertilizationRepository,
        IPlantProtectionRepository plantProtectionRepository,
        ICultivationOperationRepository cultivationOperationRepository)
    {
        _userManager = userManager;
        _fertilizationRepository = fertilizationRepository;
        _plantProtectionRepository = plantProtectionRepository;
        _cultivationOperationRepository = cultivationOperationRepository;
    }
    
    public async Task<HomePageDTO> GetHomePageInfo(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new Exception($"User not found with username: {userName}");
        }

        var cultivationOperation = await _cultivationOperationRepository.GetLatestCultivationOperationByUser(user.Id);
        var fertilization = await _fertilizationRepository.GetLatestFertilizationByUser(user.Id);
        var plantProtection =  await _plantProtectionRepository.GetLatestPlantProtectionByUser(user.Id);
        
        return new HomePageDTO
        {
            CultivationOperationName = cultivationOperation?.Name ?? "No Value",
            CultivationOperationDescription = cultivationOperation?.Description ?? "No Value",
            CultivationOperationDate = cultivationOperation?.Date,
            CultivationOperationCrop = new MiniItemDTO { Id = cultivationOperation?.Crop.Id.ToString() ?? "No Value" ,Name = cultivationOperation?.Crop?.Name ?? "No Value" },
            PlantProtectionName = plantProtection?.Type.ToString() ?? "No Value",
            PlantProtectionDescription = plantProtection?.Description ?? "No Value",
            PlantProtectionDate = plantProtection?.Date,
            PlantProtectionCrop = new MiniItemDTO { Id = plantProtection?.Crop.Id.ToString() ?? "No Value", Name = plantProtection?.Crop?.Name ?? "No Value" },
            FertilizationName = fertilization?.Type.ToString() ?? "No Value",
            FertilizationDescription = fertilization?.Description ?? "No Value",
            FertilizationDate = fertilization?.Date,
            FertilizationCrop = new MiniItemDTO { Id = fertilization?.Crop.Id.ToString() ?? "No Value", Name = fertilization?.Crop?.Name ?? "No Value" },
        };
    }
}