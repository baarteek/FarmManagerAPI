using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.DTOs
{
    public class FieldEditDTO
    {
        public Guid Id { get; set; }
        public Guid FarmId { get; set; }
        public string Name { get; set; }
        public double? Area { get; set; }
        public SoilType SoilType { get; set; }
    }
}
