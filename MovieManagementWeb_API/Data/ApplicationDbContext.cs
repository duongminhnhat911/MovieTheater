using Microsoft.EntityFrameworkCore;
using MovieManagementWeb_API.Models.Entities;

namespace MovieManagementWeb_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
