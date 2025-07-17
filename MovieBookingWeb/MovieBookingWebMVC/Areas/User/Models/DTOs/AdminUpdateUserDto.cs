namespace MovieBookingWebMVC.Areas.User.Models.DTOs
{
    public class AdminUpdateUserDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? IdCard { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Role { get; set; }
    }
}
