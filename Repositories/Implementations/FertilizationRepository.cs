using FarmManagerAPI.Data;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmManagerAPI.Repositories.Implementations
{
    public class FertilizationRepository : GenericRepository<Fertilization>, IFertilizationRepository
    {
        private readonly FarmContext _context;

        public FertilizationRepository(FarmContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Fertilization>> GetFertilizationsByCropId(Guid cropId)
        {
            return await _context.Fertilizations
                .Include(f => f.Crop)
                .Where(f => f.Crop.Id == cropId)
                .ToListAsync();
        }

        public async Task<Fertilization?> GetLatestFertilizationByUser(string userId)
        {
            return await _context.Fertilizations
                .Include(f => f.Crop)
                .ThenInclude(c => c.Field)
                .ThenInclude(fi => fi.Farm)
                .Where(f => f.Crop.Field.Farm.User.Id == userId)
                .OrderByDescending(f => f.Date)
                .FirstOrDefaultAsync();
        }

        public override async Task<Fertilization> GetById(Guid id)
        {
            return await _context.Fertilizations
                .Include(f => f.Crop)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}