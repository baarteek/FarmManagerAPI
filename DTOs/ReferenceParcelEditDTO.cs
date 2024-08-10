namespace FarmManagerAPI.DTOs
{
    public class ReferenceParcelEditDTO
    {
        public Guid FieldId { get; set; }
        public string ParcelNumber { get; set; }
        public double? Area { get; set; }
    }
}
