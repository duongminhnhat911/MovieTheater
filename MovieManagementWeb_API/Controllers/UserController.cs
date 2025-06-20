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
    }
}
