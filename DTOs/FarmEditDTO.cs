namespace FarmManagerAPI.DTOs
{
    public class FarmEditDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string? Location { get; set; }
        public double? TotalArea { get; set; }
    }
}
