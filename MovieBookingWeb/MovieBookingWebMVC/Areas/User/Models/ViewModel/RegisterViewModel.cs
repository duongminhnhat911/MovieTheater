using System.ComponentModel.DataAnnotations;

namespace MovieBookingWebMVC.Areas.User.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên tài khoản.")]
        [StringLength(50, ErrorMessage = "Tên tài khoản không được vượt quá 50 ký tự.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh.")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số CCCD.")]
        [RegularExpression(@"^\d{9,12}$", ErrorMessage = "CCCD chỉ được chứa 9 đến 12 chữ số.")]
        public string? IdCard { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^(0|\+84)[1-9][0-9]{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự.")]
        public string? Address { get; set; }
    }
}
