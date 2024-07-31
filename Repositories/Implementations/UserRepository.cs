using FarmManagerAPI.Data;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FarmManagerAPI.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly FarmContext context;

        public UserRepository(FarmContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<User> GetByEmail(string email)
        {
            return await context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }
    }
}
