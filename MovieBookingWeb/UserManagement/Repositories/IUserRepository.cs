using UserManagement.Models.DTOs;
using UserManagement.Models.Entities;

namespace UserManagement.Repositories
{
    public interface IUserRepository
    {
        Task<User?> FindUserAsync(string usernameOrEmail);
        Task<bool> AddUserAsync(RegisterDto dto);
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> UpdateUserAsync(string username, UpdateProfileDto dto);

        //Admin
        Task<List<GetListUserDto>> GetAllUsersAsync();
        Task<GetListUserDto?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserByIdAsync(int id, AdminUpdateUserDto dto);
        Task<bool> ToggleUserLockAsync(int id);
        Task<bool> UpdatePasswordAsync(User user, string newPassword);
    }
}
