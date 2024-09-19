namespace FarmManagerAPI.DTOs
{
    public class FieldDTO
    {
        public Guid Id { get; set; }
        public MiniItemDTO Farm { get; set; }
        public string Name { get; set; }
        public double? Area { get; set; }
        public string SoilType { get; set; }
        public ICollection<MiniItemDTO>? ReferenceParcels { get; set; }
        public ICollection<MiniItemDTO>? SoilMeasurements { get; set; }
        public ICollection<MiniItemDTO>? Crops { get; set; }
    }
}
