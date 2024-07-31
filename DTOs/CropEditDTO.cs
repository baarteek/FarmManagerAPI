using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.DTOs
{
    public class CropEditDTO
    {
        public Guid Id { get; set; }
        public Guid FieldId { get; set; }
        public string Name { get; set; }
        public CropType Type { get; set; }
        public DateTime? SowingDate { get; set; }
        public DateTime? HarvestDate { get; set; }
        public bool IsActive { get; set; }
    }
}
