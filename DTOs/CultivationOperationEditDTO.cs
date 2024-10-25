using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.DTOs;

public class CultivationOperationEditDTO
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public AgrotechnicalIntervention? AgrotechnicalIntervention { get; set; }
    public Guid CropId { get; set; }
}