using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Models.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Password { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? BirthDate { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string IdCard { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Role { get; set; } = "User";

        public bool IsLocked { get; set; } = false;
    }
}
