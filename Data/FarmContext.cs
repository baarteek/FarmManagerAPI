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
        public DbSet<CultivationOperation> CultivationOperations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            var userId = Guid.NewGuid().ToString();
            var user = new IdentityUser
            {
                Id = userId,
                UserName = "string",
                NormalizedUserName = "STRING",
                Email = "user@example.com",
                NormalizedEmail = "USER@EXAMPLE.COM",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            var passwordHasher = new PasswordHasher<IdentityUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, "string");

            modelBuilder.Entity<IdentityUser>().HasData(user);
        }
    }
}
