namespace FarmManagerAPI.Models
{
    public class ReferenceParcel
    {
        public int Id { get; set; }
        public Field Field { get; set; }
        public string ParcelNumber { get; set; }
        public double? Area { get; set; }
    }
}
