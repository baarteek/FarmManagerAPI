namespace FarmManagerAPI.Models
{
    public class SoilMeasurement
    {
        public int Id { get; set; }
        public Field Field { get; set; }
        public DateTime Date { get; set; }
        public double? pH { get; set; }
        public double? Nitrogen { get; set; }
        public double? Phosphorus { get; set; }
        public double? Potassium { get; set; }
    }
}
