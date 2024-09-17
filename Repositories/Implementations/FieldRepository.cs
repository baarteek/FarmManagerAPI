using FarmManagerAPI.Data;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmManagerAPI.Repositories.Implementations
{
    public class FieldRepository : GenericRepository<Field>, IFieldRepository
    {
        private readonly FarmContext _context;

        public FieldRepository(FarmContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Field>> GetFieldsByFarmId(Guid farmId)
        {
            return await _context.Fields
                .Include(f => f.Farm)
                .Where(f => f.Farm.Id == farmId)
                .ToListAsync();
        }

        public override async Task<Field> GetById(Guid id)
        {
            return await _context.Fields
                .Include(f => f.Farm)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<string> GetCoordinatesByFieldId(Guid fieldId)
        {
            var field = await _context.Fields
                .Where(f => f.Id == fieldId)
                .Select(f => f.Coordinates)
                .FirstOrDefaultAsync();
            
            return field ?? string.Empty;
        }
        
        public async Task<Guid> GetFieldIdByCoordinates(string coordinates)
        {
            return await _context.Fields
                .Where(f => f.Coordinates == coordinates)
                .Select(f => f.Id)
                .FirstOrDefaultAsync();
        }
    }
}
