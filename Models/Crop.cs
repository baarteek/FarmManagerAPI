using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.Models
{
    public class Crop
    {
        public Guid Id { get; set; }
        public string? CropIdentifier { get; set; }
        public Field Field { get; set; }
        public string Name { get; set; }
        public CropType Type { get; set; }
        public bool IsActive { get; set; }
        public ICollection<CultivationOperation>? CultivationOperations { get; set; }
        public ICollection<Fertilization>? Fertilizations { get; set; }
        public ICollection<PlantProtection>? PlantProtections { get; set; }
    }
}