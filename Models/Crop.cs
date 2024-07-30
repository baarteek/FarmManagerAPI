using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.Models
{
    public class Crop
    {
        public int Id { get; set; }
        public Field Field { get; set; }
        public string Name { get; set; }
        public CropType Type { get; set; }
        public DateTime? SowingDate { get; set; }
        public DateTime? HarvestDate { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Fertilization>? Fertilizations { get; set; }
        public ICollection<PlantProtection>? PestAndDiseases { get; set; }
    }
}