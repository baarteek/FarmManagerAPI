namespace FarmManagerAPI.DTOs
{
    public class SoilMeasurementEditDTO
    {
        public Guid Id { get; set; }
        public Guid FieldId { get; set; }
        public DateTime Date { get; set; }
        public double? pH { get; set; }
        public double? Nitrogen { get; set; }
        public double? Phosphorus { get; set; }
        public double? Potassium { get; set; }
    }
}
