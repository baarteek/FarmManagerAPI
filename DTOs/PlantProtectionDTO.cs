using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.DTOs
{
    public class PlantProtectionDTO
    {
        public Guid Id { get; set; }
        public MiniItemDTO Crop { get; set; }
        public DateTime Date { get; set; }
        public PlantProtectionType Type { get; set; }
        public string? AgrotechnicalIntervention { get; set; }
        public string? AgrotechicalIntervationDescription { get; set; }
        public string? NameOfProduct { get; set; }
        public double? Quantity { get; set; }
        public string? Description { get; set; }
    }
}
