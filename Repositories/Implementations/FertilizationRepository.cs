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

        public override async Task<Fertilization> GetById(Guid id)
        {
            return await _context.Fertilizations
                .Include(f => f.Crop)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}