using FarmManagerAPI.Data;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;

namespace FarmManagerAPI.Repositories.Implementations
{
    public class CropRepository : GenericRepository<Crop>, ICropRepository
    {
        private readonly FarmContext context;

        public CropRepository(FarmContext context) : base(context)
        {
            this.context = context;
        }
    }
}
