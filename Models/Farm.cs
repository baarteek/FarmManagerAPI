using Microsoft.AspNetCore.Identity;

namespace FarmManagerAPI.Models
{
    public class Farm
    {
        public Guid Id { get; set; }
        public IdentityUser User { get; set; }
        public string Name { get; set; }
        public string? Location { get; set; }
        public double? TotalArea { get; set; }
        public ICollection<Field>? Fields { get; set; }
    }
}
