using AutoMapper;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace FarmManagerAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<IdentityUser, UserDTO>().ReverseMap();
            CreateMap<IdentityUser, UserEditDTO>().ReverseMap();
            CreateMap<Crop, CropDTO>().ReverseMap();
            CreateMap<Crop, CropEditDTO>().ReverseMap();
            CreateMap<Farm, FarmDTO>().ReverseMap();
            CreateMap<Farm, FarmEditDTO>().ReverseMap();
            CreateMap<Fertilization, FertilizationDTO>().ReverseMap();
            CreateMap<Fertilization, FertilizationEditDTO>().ReverseMap();
            CreateMap<Field, FieldDTO>().ReverseMap();
            CreateMap<Field, FieldEditDTO>().ReverseMap();
            CreateMap<PlantProtection, PlantProtectionDTO>().ReverseMap();
            CreateMap<PlantProtection, PlantProtectionEditDTO>().ReverseMap();
            CreateMap<ReferenceParcel, ReferenceParcelDTO>().ReverseMap();
            CreateMap<ReferenceParcel, ReferenceParcelEditDTO>().ReverseMap();
            CreateMap<SoilMeasurement, SoilMeasurementDTO>().ReverseMap();
            CreateMap<SoilMeasurement, SoilMeasurementEditDTO>().ReverseMap();
        }
    }
}
