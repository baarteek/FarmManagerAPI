namespace FarmManagerAPI.DTOs;

public class MapDataDTO
{
    public Guid FieldId { get; set; }
    public string FieldName { get; set; }
    public double? Area { get; set; }
    public string? Coordinates { get; set; }
    public Guid CropId { get; set; }
    public string CropName { get; set; }
}