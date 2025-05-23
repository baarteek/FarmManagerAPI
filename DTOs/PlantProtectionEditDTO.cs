﻿using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.DTOs
{
    public class PlantProtectionEditDTO
    {
        public Guid CropId { get; set; }
        public DateTime Date { get; set; }
        public PlantProtectionType Type { get; set; }
        public AgrotechnicalIntervention? AgrotechnicalIntervention { get; set; }
        public string? NameOfProduct { get; set; }
        public double? Quantity { get; set; }
        public string? Description { get; set; }
    }
}
