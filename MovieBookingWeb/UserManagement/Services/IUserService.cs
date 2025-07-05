using UserManagement.Models.DTOs;
using UserManagement.Models.Entities;

namespace UserManagement.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(LoginDto dto);
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<UpdateProfileDto?> GetProfileAsync(string username);
        Task<bool> UpdateProfileAsync(string username, UpdateProfileDto dto);

        //admin
        Task<List<GetListUserDto>> GetAllUsersAsync();
        Task<GetListUserDto?> GetUserByIdAsync(int id);
        Task<bool> AdminUpdateUserAsync(int id, AdminUpdateUserDto dto);
        Task<bool> ToggleUserLockAsync(int id);
        Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword);
    }
}
