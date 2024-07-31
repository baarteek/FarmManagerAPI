using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;

namespace FarmManagerAPI.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByEmail(string email);
    }
}
