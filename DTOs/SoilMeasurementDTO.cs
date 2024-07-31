namespace FarmManagerAPI.DTOs
{
    public class SoilMeasurementDTO
    {
        public Guid Id { get; set; }
        public MiniItemDTO Field { get; set; }
        public DateTime Date { get; set; }
        public double? pH { get; set; }
        public double? Nitrogen { get; set; }
        public double? Phosphorus { get; set; }
        public double? Potassium { get; set; }
    }
}
