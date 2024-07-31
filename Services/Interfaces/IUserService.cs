using FarmManagerAPI.DTOs;

namespace FarmManagerAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetUserById(Guid id);
        Task<UserDTO> GetUserByEmail(string email);
        Task AddUser(UserEditDTO user);
        Task UpdateUser(Guid id, UserEditDTO user);
        Task DeleteUser(Guid id);
    }
}
