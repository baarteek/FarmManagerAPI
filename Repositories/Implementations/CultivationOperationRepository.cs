using FarmManagerAPI.Data;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmManagerAPI.Repositories.Implementations;

public class CultivationOperationRepository : GenericRepository<CultivationOperation>, ICultivationOperationRepository
{
    private readonly FarmContext _context;

    public CultivationOperationRepository(FarmContext context) : base(context)
    {
        _context = context;
    }
    
    public override async Task<CultivationOperation> GetById(Guid id)
    {
        return await _context.CultivationOperations
            .Include(c => c.Crop)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<CultivationOperation>> GetCultivationOperationsByCropId(Guid cropId)
    {
        return await _context.CultivationOperations
            .Include(c => c.Crop)
            .Where(c => c.Crop.Id == cropId)
            .ToListAsync();
    }

    public async Task<CultivationOperation?> GetLatestCultivationOperationByUser(string userId)
    {
        return await _context.CultivationOperations
            .Include(co => co.Crop)
            .ThenInclude(c => c.Field)
            .ThenInclude(f => f.Farm)
            .Where(co => co.Crop.Field.Farm.User.Id == userId)
            .OrderByDescending(co => co.Date)
            .FirstOrDefaultAsync();
    }
}