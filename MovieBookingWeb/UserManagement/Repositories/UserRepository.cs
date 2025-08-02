using Microsoft.EntityFrameworkCore;
using UserManagement.Models.DTOs;
using UserManagement.Models.Entities;

namespace UserManagement.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<User?> FindUserAsync(string usernameOrEmail)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == usernameOrEmail || u.Email == usernameOrEmail);
        }

        public async Task<bool> AddUserAsync(RegisterDto dto)
        {
            bool exists = await _context.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
            if (exists) return false;

            var user = new User
            {
                Username = dto.Username,
                Password = dto.Password,
                FullName = dto.FullName,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender,
                Email = dto.Email,
                IdCard = dto.IdCard,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Role = "User",
                IsLocked = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> UpdateUserAsync(string username, UpdateProfileDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return false;

            user.Username = dto.Username;
            user.FullName = dto.FullName;
            user.BirthDate = dto.BirthDate;
            user.Gender = dto.Gender;
            user.Email = dto.Email;
            user.IdCard = dto.IdCard;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<GetListUserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new GetListUserDto
                {
                    Id = u.Id,
                    Username = u.Username!,
                    FullName = u.FullName!,
                    Email = u.Email!,
                    Role = u.Role,
                    IsLocked = u.IsLocked,
                    BirthDate = u.BirthDate,
                    Gender = u.Gender,
                    IdCard = u.IdCard,
                    PhoneNumber = u.PhoneNumber,
                    Address = u.Address
                })
                .ToListAsync();
        }

        public async Task<GetListUserDto?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new GetListUserDto
                {
                    Id = u.Id,
                    Username = u.Username!,
                    FullName = u.FullName!,
                    Email = u.Email!,
                    Role = u.Role,
                    IsLocked = u.IsLocked,
                    BirthDate = u.BirthDate,
                    Gender = u.Gender,
                    IdCard = u.IdCard,
                    PhoneNumber = u.PhoneNumber,
                    Address = u.Address
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateUserByIdAsync(int id, AdminUpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.Username = dto.Username;
            user.FullName = dto.FullName;
            user.BirthDate = dto.BirthDate;
            user.Gender = dto.Gender;
            user.Email = dto.Email;
            user.IdCard = dto.IdCard;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;
            user.Role = dto.Role;
            user.IsLocked = dto.IsLocked;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleUserLockAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            if (user.Role == "Admin")
                return false;

            user.IsLocked = !user.IsLocked;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdatePasswordAsync(User user, string newPassword)
        {
            user.Password = newPassword;
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }

}

