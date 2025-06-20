using System.ComponentModel.DataAnnotations;

namespace MovieBookingWeb.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản.")] public string? Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)] public string? Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        [DataType(DataType.Password)] public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")] public string? FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh.")]
        [DataType(DataType.Date)] public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính.")] public string? Gender { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")] public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số CCCD.")] public string? IdCard { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")] public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")] public string? Address { get; set; }
    }
}
