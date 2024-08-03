using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace FarmManagerAPI.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return;

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Could not delete user");
            }
        }

        public async Task<UserDTO> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            return new UserDTO
            {
                Id = Guid.Parse(user.Id),
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<UserDTO> GetUserById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return null;

            return new UserDTO
            {
                Id = Guid.Parse(user.Id),
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task UpdateUser(Guid id, UserEditDTO userEditDto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return;

            user.Email = userEditDto.Email;
            user.UserName = userEditDto.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Could not update user");
            }
        }
    }
}
