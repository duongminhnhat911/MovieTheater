namespace MovieManagementWeb_API.Models.DTOs
{
    public class AdminUpdateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IdCard { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public bool IsLocked { get; set; } = false;
    }
}
