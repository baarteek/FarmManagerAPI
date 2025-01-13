namespace FarmManagerAPI.DTOs;

public class HomePageDTO
{
        public string? CultivationOperationName { get; set; }
        public string? CultivationOperationDescription { get; set; }
        public DateTime? CultivationOperationDate { get; set; }
        public MiniItemDTO? CultivationOperationCrop { get; set; }
        public string? PlantProtectionName { get; set; }
        public string? PlantProtectionDescription { get; set; }
        public DateTime? PlantProtectionDate { get; set; }
        public MiniItemDTO? PlantProtectionCrop { get; set; }
        public string? FertilizationName { get; set; }
        public string? FertilizationDescription { get; set; }
        public DateTime? FertilizationDate { get; set; }
        public MiniItemDTO? FertilizationCrop { get; set; }
}