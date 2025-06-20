using Microsoft.EntityFrameworkCore;
using MovieManagementWeb_API.Models.Entities;
using MovieManagementWeb_API.Data;

namespace MovieManagementWeb_API
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (context.Users.Any()) return;

            var users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    Password = "admin123",
                    FullName = "Admin User",
                    BirthDate = new DateTime(1990, 1, 1),
                    Gender = "Male",
                    Email = "admin@example.com",
                    IdCard = "0123456789",
                    PhoneNumber = "0901234567",
                    Address = "Admin Street",
                    Role = "Admin",
                    IsLocked = false
                },
                new User
                {
                    Username = "employee",
                    Password = "emp123",
                    FullName = "Employee User",
                    BirthDate = new DateTime(1995, 5, 5),
                    Gender = "Female",
                    Email = "employee@example.com",
                    IdCard = "9876543210",
                    PhoneNumber = "0909876543",
                    Address = "Employee Street",
                    Role = "Employee",
                    IsLocked = false
                },
                new User
                {
                    Username = "user1",
                    Password = "user123",
                    FullName = "Normal User",
                    BirthDate = new DateTime(2000, 12, 12),
                    Gender = "Male",
                    Email = "user@example.com",
                    IdCard = "1122334455",
                    PhoneNumber = "0911223344",
                    Address = "User Street",
                    Role = "User",
                    IsLocked = false
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
