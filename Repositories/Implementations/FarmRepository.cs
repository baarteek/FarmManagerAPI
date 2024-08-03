using FarmManagerAPI.Data;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmManagerAPI.Repositories.Implementations
{
    public class FarmRepository : GenericRepository<Farm>, IFarmRepository
    {
        private readonly FarmContext context;

        public FarmRepository(FarmContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Farm>> GetFarmsByUser(string userId)
        {
            return await context.Farms
                .Include(f => f.User)
                .Where(f => f.User.Id == userId)
                .ToListAsync();
        }

        public override async Task<Farm> GetById(Guid id)
        {
            return await context.Farms
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
