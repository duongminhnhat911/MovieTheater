using System.ComponentModel.DataAnnotations;

namespace MovieBookingWebMVC.Areas.User.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email hoặc tên tài khoản.")]
        public string? UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
