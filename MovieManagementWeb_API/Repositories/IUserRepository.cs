using MovieManagementWeb_API.Models.DTOs;
using MovieManagementWeb_API.Models.Entities;

namespace MovieManagementWeb_API.Repositories
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
    }
}
