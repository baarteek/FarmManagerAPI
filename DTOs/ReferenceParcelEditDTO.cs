namespace FarmManagerAPI.DTOs
{
    public class ReferenceParcelEditDTO
    {
        public Guid Id { get; set; }
        public Guid FiledId { get; set; }
        public string ParcelNumber { get; set; }
        public double? Area { get; set; }
    }
}
