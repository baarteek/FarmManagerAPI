using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.DTOs
{
    public class CropEditDTO
    {
        public Guid FieldId { get; set; }
        public string Name { get; set; }
        public CropType Type { get; set; }
        public bool IsActive { get; set; }
    }
}
