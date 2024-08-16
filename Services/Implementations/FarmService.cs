using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FarmManagerAPI.Services.Implementations
{
    public class FarmService : IFarmService
    {
        private readonly IFarmRepository _farmRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFieldRepository _fieldRepository;

        public FarmService(IFarmRepository farmRepository, UserManager<IdentityUser> userManager, IFieldRepository fieldRepository)
        {
            _farmRepository = farmRepository;
            _userManager = userManager;
            _fieldRepository = fieldRepository;
        }

        public async Task<FarmDTO> AddFarm(FarmEditDTO farmEditDto, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new Exception($"User not found with username: {userName}");
            }

            var farm = new Farm
            {
                Id = Guid.NewGuid(),
                User = user,
                Name = farmEditDto.Name,
                Location = farmEditDto.Location,
                TotalArea = farmEditDto.TotalArea
            };

            await _farmRepository.Add(farm);

            return new FarmDTO
            {
                Id = farm.Id,
                User = new MiniItemDTO { Id = farm.User.Id, Name = farm.User.UserName },
                Name = farmEditDto.Name,
                Location = farm.Location,
                TotalArea = farm.TotalArea,
                Fields = new List<MiniItemDTO>()
            };
        }

        public async Task DeleteFarm(Guid id)
        {
            await _farmRepository.Delete(id);
        }

        public async Task<FarmDTO> GetFarmById(Guid id)
        {
            var farm = await _farmRepository.GetById(id);
            if (farm == null)
            {
                throw new Exception($"Farm not found with ID: {id}");
            }

            if (farm.User == null)
            {
                throw new Exception($"User is not loaded for farm with ID: {id}");
            }

            var user = await _userManager.FindByIdAsync(farm.User.Id);
            if (user == null)
            {
                throw new Exception($"User not found with ID: {farm.User.Id}");
            }

            farm.Fields = (await _fieldRepository.GetFieldsByFarmId(farm.Id)).ToList();

            return new FarmDTO
            {
                Id = farm.Id,
                User = new MiniItemDTO { Id = user.Id, Name = user.UserName },
                Name = farm.Name,
                Location = farm.Location,
                TotalArea = farm.TotalArea,
                Fields = farm.Fields != null ? farm.Fields.Select(f => new MiniItemDTO { Id = f.Id.ToString(), Name = f.Name }).ToList() : new List<MiniItemDTO>()
            };
        }

        public async Task<IEnumerable<FarmDTO>> GetFarmsByUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new Exception($"User not found with username: {userName}");
            }

            var farms = await _farmRepository.GetFarmsByUser(user.Id);

            foreach(var farm in farms)
            {
                farm.Fields = (await _fieldRepository.GetFieldsByFarmId(farm.Id)).ToList();
            }

            return farms.Select(farm => new FarmDTO
            {
                Id = farm.Id,
                User = new MiniItemDTO { Id = farm.User.Id, Name = farm.User.UserName },
                Name = farm.Name,
                Location = farm.Location,
                TotalArea = farm.TotalArea,
                Fields = farm.Fields != null ? farm.Fields.Select(f => new MiniItemDTO { Id = f.Id.ToString(), Name = f.Name }).ToList() : new List<MiniItemDTO>()
            });
        }

        public async Task UpdateFarm(Guid id, FarmEditDTO farmEditDto)
        {
            var farm = await _farmRepository.GetById(id);
            if (farm == null) return;

            farm.Name = farmEditDto.Name;
            farm.Location = farmEditDto.Location;
            farm.TotalArea = farmEditDto.TotalArea;

            await _farmRepository.Update(farm);
        }
    }
}
