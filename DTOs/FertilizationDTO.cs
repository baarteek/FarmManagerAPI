using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.DTOs
{
    public class FertilizationDTO
    {
        public Guid Id { get; set; }
        public MiniItemDTO Crop { get; set; }
        public DateTime Date { get; set; }
        public FertilizationType Type { get; set; }
        public string? NameOfProduct { get; set; }
        public double? Quantity { get; set; }
        public string? Description { get; set; }
    }
}
