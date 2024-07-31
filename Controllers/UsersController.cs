using FarmManagerAPI.DTOs;
using FarmManagerAPI.Services.Implementations;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmManagerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(Guid id)
        {
            var user = await userService.GetUserById(id);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserDTO>> GetUserByEmail(string email)
        {
            var user = await userService.GetUserByEmail(email);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult> AddUser(UserEditDTO userEditDTO)
        {
            try
            {
                await userService.AddUser(userEditDTO);
                var user = await userService.GetUserByEmail(userEditDTO.Email);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userEditDTO);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(Guid id, UserEditDTO userEditDTO)
        {
            try
            {
                await userService.UpdateUser(id, userEditDTO);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            await userService.DeleteUser(id);
            return NoContent();
        }
    }
}
