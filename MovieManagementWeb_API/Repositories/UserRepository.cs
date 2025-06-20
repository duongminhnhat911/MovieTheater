using Microsoft.EntityFrameworkCore;
using MovieManagementWeb_API.Data;
using MovieManagementWeb_API.Models.DTOs;
using MovieManagementWeb_API.Models.Entities;

namespace MovieManagementWeb_API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> FindUserAsync(string usernameOrEmail, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u =>
                    (u.Username == usernameOrEmail || u.Email == usernameOrEmail) &&
                    u.Password == password);
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

    }
}
