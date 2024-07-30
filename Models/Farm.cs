namespace FarmManagerAPI.Models
{
    public class Farm
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public string? Location { get; set; }
        public double? TotalArea { get; set; }
        public ICollection<Field>? Fields { get; set; }
    }
}
