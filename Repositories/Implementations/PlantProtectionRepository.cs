using FarmManagerAPI.Data;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmManagerAPI.Repositories.Implementations
{
    public class PlantProtectionRepository : GenericRepository<PlantProtection>, IPlantProtectionRepository
    {
        private readonly FarmContext _context;

        public PlantProtectionRepository(FarmContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PlantProtection>> GetPlantProtectionsByCropId(Guid cropId)
        {
            return await _context.PlantProtection
                .Include(pp => pp.Crop)
                .Where(pp => pp.Crop.Id == cropId)
                .ToListAsync();
        }

        public async Task<PlantProtection?> GetLatestPlantProtectionByUser(string userId)
        {
            return await _context.PlantProtection
                .Include(pp => pp.Crop)
                .ThenInclude(c => c.Field)
                .ThenInclude(f => f.Farm)
                .Where(pp => pp.Crop.Field.Farm.User.Id == userId)
                .OrderByDescending(pp => pp.Date)
                .FirstOrDefaultAsync();
        }

        public override async Task<PlantProtection> GetById(Guid id)
        {
            return await _context.PlantProtection
                .Include(pp => pp.Crop)
                .FirstOrDefaultAsync(pp => pp.Id == id);
        }
    }
}
