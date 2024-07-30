namespace FarmManagerAPI.Models
{
    public class Crop
    {
        public int Id { get; set; }
        public Field Field { get; set; }
        public string Type { get; set; }
        public DateTime? SowingDate { get; set; }
        public DateTime? HarvestDate { get; set; }
        public bool isActive { get; set; }
        public ICollection<Fertilization>? Fertilizations { get; set; }
        public ICollection<PestAndDisease>? PestAndDiseases { get; set; }
    }
}