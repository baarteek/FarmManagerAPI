﻿using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.DTOs
{
    public class FertilizationEditDTO
    {
        public Guid Id { get; set; }
        public Guid CropId { get; set; }
        public DateTime Date { get; set; }
        public FertilizationType Type { get; set; }
        public double? Quantity { get; set; }
        public string? Method { get; set; }
        public string? Description { get; set; }
    }
}
