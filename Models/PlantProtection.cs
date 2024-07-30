using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.Models
{
    public class PlantProtection
    {
        public int Id { get; set; }
        public Crop Crop { get; set; }
        public DateTime Date { get; set; }
        public PlantProtectionType Type { get; set; }
        public double? Quantity { get; set; }
        public string? Method { get; set; }
        public string? Description {  get; set; }
    }
}
