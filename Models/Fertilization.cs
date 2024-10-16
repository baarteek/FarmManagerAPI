﻿using FarmManagerAPI.Models.Enums;

namespace FarmManagerAPI.Models
{
    public class Fertilization
    {
        public Guid Id { get; set; }
        public Crop Crop { get; set; }
        public DateTime Date { get; set; }
        public FertilizationType Type { get; set; }
        public string? NameOfProduct { get; set; }
        public double? Quantity { get; set; }
        public string? Method { get; set; }
        public string? Description { get; set; }
    }
}
