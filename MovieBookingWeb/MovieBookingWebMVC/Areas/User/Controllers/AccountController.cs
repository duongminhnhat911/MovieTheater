using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBookingWebMVC.Areas.User.Models.DTOs;
using MovieBookingWebMVC.Areas.User.Models.ViewModel;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;

namespace MovieBookingWebMVC.Areas.User.Controllers
{
    [Area("User")]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _contextAccessor;

        public AccountController(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("ApiClient_User");
            var response = await client.PostAsJsonAsync("api/auth/login", model);

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                ModelState.AddModelError("", "Tài khoản đã bị khóa. Vui lòng liên hệ quản trị viên.");
                return View(model);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu.");
                return View(model);
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Đăng nhập thất bại. Vui lòng thử lại sau.");
                return View(model);
            }

            var userJson = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserDto>(userJson);

            if (user == null)
            {
                ModelState.AddModelError("", "Lỗi không xác định từ phía máy chủ.");
                return View(model);
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim("Username", user.Username),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (user.Role == "Admin")
            {
                return RedirectToAction("ViewMovie", "Movie", new { area = "Movie" });
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }


        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("ApiClient_User");
            var response = await client.PostAsJsonAsync("api/auth/register", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Đăng ký thất bại. Có thể tài khoản/email đã tồn tại.");
                return View(model);
            }

            TempData["Success"] = "Đăng ký thành công, vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var client = _httpClientFactory.CreateClient("ApiClient_User");

            // Đính kèm cookie từ MVC sang Web API
            var cookie = _contextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
            if (!string.IsNullOrEmpty(cookie))
                client.DefaultRequestHeaders.Add("Cookie", cookie);

            var response = await client.GetAsync("api/user/profile");
            if (!response.IsSuccessStatusCode) return RedirectToAction("Login");

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<UpdateProfileViewModel>(json);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid) return View("Profile", model);

            var client = _httpClientFactory.CreateClient("ApiClient_User");
            var cookie = _contextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
            if (!string.IsNullOrEmpty(cookie))
                client.DefaultRequestHeaders.Add("Cookie", cookie);

            var response = await client.PutAsJsonAsync("api/user/profile", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Cập nhật thất bại.");
                return View(model);
            }

            TempData["Success"] = "Cập nhật thành công!";
            return RedirectToAction("Profile");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() => View();

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Xác nhận mật khẩu không khớp.");
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("ApiClient_User");

            var cookie = _contextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
            if (!string.IsNullOrEmpty(cookie))
                client.DefaultRequestHeaders.Add("Cookie", cookie);

            var dto = new
            {
                oldPassword = model.OldPassword,
                newPassword = model.NewPassword
            };

            var response = await client.PostAsJsonAsync("api/user/change-password", dto);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Đổi mật khẩu thất bại. Mật khẩu hiện tại không đúng?");
                return View(model);
            }

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Profile");
        }
    }
}