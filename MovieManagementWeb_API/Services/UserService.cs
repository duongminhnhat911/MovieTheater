using MovieManagementWeb_API.Models.DTOs;
using MovieManagementWeb_API.Models.Entities;
using MovieManagementWeb_API.Repositories;

namespace MovieManagementWeb_API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<User?> AuthenticateAsync(LoginDto dto)
        {
            return await _repo.FindUserAsync(dto.UsernameOrEmail);
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            return await _repo.AddUserAsync(dto);
        }

        public async Task<UpdateProfileDto?> GetProfileAsync(string username)
        {
            var user = await _repo.GetByUsernameAsync(username);
            return user == null ? null : new UpdateProfileDto
            {
                Username = user.Username,
                FullName = user.FullName,
                BirthDate = (DateTime)user.BirthDate,
                Gender = user.Gender,
                Email = user.Email,
                IdCard = user.IdCard,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };
        }

        public async Task<bool> UpdateProfileAsync(string username, UpdateProfileDto dto)
        {
            return await _repo.UpdateUserAsync(username, dto);
        }

        //Admin
        public async Task<List<GetListUserDto>> GetAllUsersAsync()
        => await _repo.GetAllUsersAsync();

        public async Task<GetListUserDto?> GetUserByIdAsync(int id)
            => await _repo.GetUserByIdAsync(id);

        public async Task<bool> AdminUpdateUserAsync(int id, AdminUpdateUserDto dto)
            => await _repo.UpdateUserByIdAsync(id, dto);

        public async Task<bool> ToggleUserLockAsync(int id)
        {
            return await _repo.ToggleUserLockAsync(id);
        }
    }
}
