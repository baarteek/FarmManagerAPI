namespace FarmManagerAPI.DTOs;

public class CultivationOperationDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public MiniItemDTO Crop { get; set; }
}