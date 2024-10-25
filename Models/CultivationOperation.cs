using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.Models;

public class CultivationOperation
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public AgrotechnicalIntervention? AgrotechnicalIntervention { get; set; }
    public Crop Crop { get; set; }
}