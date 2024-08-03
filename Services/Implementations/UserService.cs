using AutoMapper;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FarmManagerAPI.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;

        public UserService(UserManager<IdentityUser> userManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task DeleteUser(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null) return;

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Could not delete user");
            }
        }

        public async Task<UserDTO> GetUserByEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            return mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetUserById(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            return mapper.Map<UserDTO>(user);
        }

        public async Task UpdateUser(Guid id, UserEditDTO userEditDto)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null) return;

            user.Email = userEditDto.Email;
            user.UserName = userEditDto.UserName;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Could not update user");
            }
        }
    }
}
