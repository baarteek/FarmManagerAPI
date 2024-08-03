using FarmManagerAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FarmManagerAPI.Data
{
    public class FarmContext : IdentityDbContext<IdentityUser>
    {
        public FarmContext(DbContextOptions<FarmContext> options) : base(options) { }

        public DbSet<Farm> Farms { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<Crop> Crops { get; set; }
        public DbSet<Fertilization> Fertilizations { get; set; }
        public DbSet<PlantProtection> PlantProtection { get; set; }
        public DbSet<ReferenceParcel> ReferenceParcels { get; set; }
        public DbSet<SoilMeasurement> SoilMeasurements { get; set; }
    }
}
