using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieManagementWeb_API.Models.DTOs;
using MovieManagementWeb_API.Services;

namespace MovieManagementWeb_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.Identity?.Name;
            var profile = await _userService.GetProfileAsync(username!);
            return profile == null ? NotFound() : Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var username = User.Identity?.Name;
            var result = await _userService.UpdateProfileAsync(username!, dto);
            return result ? Ok("Profile updated") : BadRequest("Update failed");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AdminUpdateUserDto dto)
        {
            var updated = await _userService.AdminUpdateUserAsync(id, dto);
            return updated ? Ok("Updated") : NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var toggled = await _userService.ToggleUserLockAsync(id);
            return toggled ? Ok("Toggled lock status") : NotFound("User not found");
        }


    }
}

