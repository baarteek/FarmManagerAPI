namespace FarmManagerAPI.DTOs
{
    public class FarmDTO
    {
        public Guid Id { get; set; }
        public MiniItemDTO User { get; set; }
        public string Name { get; set; }
        public string? Location { get; set; }
        public double? TotalArea { get; set; }
        public ICollection<MiniItemDTO> Fields { get; set; }
    }
}
