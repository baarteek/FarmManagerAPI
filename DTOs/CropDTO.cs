using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.DTOs
{
    public class CropDTO
    {
        public Guid Id { get; set; }
        public MiniItemDTO Field { get; set; }
        public string Name { get; set; }
        public CropType Type { get; set; }
        public DateTime? SowingDate { get; set; }
        public DateTime? HarvestDate { get; set; }
        public bool IsActive { get; set; }
        public ICollection<MiniItemDTO>? Fertilizations { get; set; }
        public ICollection<MiniItemDTO>? PlantProtections { get; set; }
    }
}
