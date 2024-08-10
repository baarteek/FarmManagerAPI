using FarmManagerAPI.Data;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmManagerAPI.Repositories.Implementations
{
    public class SoilMeasurementRepository : GenericRepository<SoilMeasurement>, ISoilMeasurementRepository
    {
        private readonly FarmContext _context;

        public SoilMeasurementRepository(FarmContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SoilMeasurement>> GetSoilMeasurementsByFieldId(Guid fieldId)
        {
            return await _context.SoilMeasurements
                .Include(sm => sm.Field)
                .Where(sm => sm.Field.Id == fieldId)
                .ToListAsync();
        }

        public override async Task<SoilMeasurement> GetById(Guid id)
        {
            return await _context.SoilMeasurements
                .Include(sm => sm.Field)
                .FirstOrDefaultAsync(sm => sm.Id == id);
        }
    }
}
