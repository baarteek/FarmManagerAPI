using FarmManagerAPI.Data;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmManagerAPI.Repositories.Implementations
{
    public class ReferenceParcelRepository : GenericRepository<ReferenceParcel>, IReferenceParcelRepository
    {
        private readonly FarmContext _context;

        public ReferenceParcelRepository(FarmContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReferenceParcel>> GetParcelsByFieldId(Guid fieldId)
        {
            return await _context.ReferenceParcels
                .Include(rp => rp.Field)
                .Where(rp => rp.Field.Id == fieldId)
                .ToListAsync();
        }

        public override async Task<ReferenceParcel> GetById(Guid id)
        {
            return await _context.ReferenceParcels
                .Include(rp => rp.Field)
                .FirstOrDefaultAsync(rp => rp.Id == id);
        }
    }
}
