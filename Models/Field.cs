using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.Models
{
    public class Field
    {
        public int Id { get; set; }
        public Farm Farm { get; set; }
        public string Name { get; set; }
        public double? Area { get; set; }
        public SoilType SoilType { get; set; }
        public ICollection<ReferenceParcel>? ReferenceParcels { get; set; }
        public ICollection<SoilMeasurement>? SoilMeasurements { get; set; }
        public ICollection<Crop>? Crops { get; set; }
    }
}
