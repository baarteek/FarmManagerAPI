namespace FarmManagerAPI.Models
{
    public class PestAndDisease
    {
        public int Id { get; set; }
        public Crop Crop { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public double? Quantity { get; set; }
        public string? Method { get; set; }
        public string? Description {  get; set; }
    }
}
