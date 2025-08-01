using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models.DTOs;
using UserManagement.Services;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AdminUpdateUserDto dto)
        {
            var updated = await _userService.AdminUpdateUserAsync(id, dto);
            return updated ? Ok("Updated") : NotFound();
        }

/*        [Authorize(Roles = "Admin")]*/
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var toggled = await _userService.ToggleUserLockAsync(id);
            return toggled ? Ok("Toggled lock status") : NotFound("User not found");
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var result = await _userService.ChangePasswordAsync(username, dto.OldPassword, dto.NewPassword);
            if (!result)
                return BadRequest("Mật khẩu hiện tại không chính xác");

            return Ok("Đổi mật khẩu thành công");
        }
    }
}