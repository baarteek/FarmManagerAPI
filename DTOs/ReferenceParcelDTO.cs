namespace FarmManagerAPI.DTOs
{
    public class ReferenceParcelDTO
    {
        public Guid Id { get; set; }
        public MiniItemDTO Field { get; set; }
        public string ParcelNumber { get; set; }
        public double? Area { get; set; }
    }
}
