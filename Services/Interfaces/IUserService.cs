using FarmManagerAPI.DTOs;

namespace FarmManagerAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetUserById(Guid id);
        Task<UserDTO> GetUserByEmail(string email);
        Task AddUser(UserEditDTO userEditDto);
        Task UpdateUser(Guid id, UserEditDTO userEditDto);
        Task DeleteUser(Guid id);
    }
}
