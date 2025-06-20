using MovieManagementWeb_API.Models.DTOs;
using MovieManagementWeb_API.Models.Entities;

namespace MovieManagementWeb_API.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(LoginDto dto);
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<UpdateProfileDto?> GetProfileAsync(string username);
        Task<bool> UpdateProfileAsync(string username, UpdateProfileDto dto);
    }
}
