using AutoMapper;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Implementations;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;

namespace FarmManagerAPI.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task AddUser(UserEditDTO userEditDto)
        {
            var existingUser = await userRepository.GetByEmail(userEditDto.Email);
            if(existingUser != null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var user = mapper.Map<User>(userEditDto);
            user.Id = Guid.NewGuid();
            await userRepository.Add(user);
        }

        public async Task DeleteUser(Guid id)
        {
            await userRepository.Delete(id);
        }

        public async Task<UserDTO> GetUserByEmail(string email)
        {
            var user = await userRepository.GetByEmail(email);
            return mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetUserById(Guid id)
        {
            var user = await userRepository.GetById(id);
            return mapper.Map<UserDTO>(user);
        }

        public async Task UpdateUser(Guid id, UserEditDTO userEditDto)
        {
            var existingUser = await userRepository.GetByEmail(userEditDto.Email);
            if (existingUser != null && existingUser.Id != id)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var user = await userRepository.GetById(id);
            if (user == null) return;

            mapper.Map(userEditDto, user);
            await userRepository.Update(user);
        }
    }
}
