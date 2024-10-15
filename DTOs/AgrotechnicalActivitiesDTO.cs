namespace FarmManagerAPI.DTOs
{
    public class AgrotechnicalActivitiesDTO
    {
        public string? CropIdentifier { get; set; }
        public string? PlotNumber { get; set; }
        public DateTime? Date { get; set; }
        public double? Area { get; set; }
        public string? TypeOfUse { get; set; }
        public string? TypeOfActivity { get; set; }
        public string? NameOfPlantProtectionProduct { get; set; }
        public string? AmountOfPlatnProtectionProduct { get; set; }
        public string? PackageNumber { get; set; }
        public string? Comments { get; set; }
    }
}
