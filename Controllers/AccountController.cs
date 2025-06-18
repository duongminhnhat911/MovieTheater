using Microsoft.AspNetCore.Mvc;
using MovieBookingWeb.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MovieBookingWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                (u.Username == model.UsernameOrEmail || u.Email == model.UsernameOrEmail)
                && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Sai tên tài khoản/mật khẩu, vui lòng nhập lại.");
                return View(model);
            }

            if (user.IsLocked)
            {
                ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (user.Role == "Admin") return RedirectToAction("Dashboard", "Admin");
            else if (user.Role == "Employee") return RedirectToAction("Dashboard", "Employee");
            else return RedirectToAction("Profile", "Account");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var exists = await _context.Users.AnyAsync(u => u.Username == model.Username || u.Email == model.Email);
            if (exists)
            {
                ModelState.AddModelError("", "Tên tài khoản hoặc E-mail đã tồn tại.");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Password = model.Password,
                FullName = model.FullName,
                BirthDate = model.BirthDate,
                Gender = model.Gender,
                Email = model.Email,
                IdCard = model.IdCard,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                Role = "User",
                IsLocked = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đăng ký thành công. Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return RedirectToAction("Login");

            var model = new UpdateProfileViewModel
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                BirthDate = user.BirthDate,
                Gender = user.Gender,
                Email = user.Email,
                IdCard = user.IdCard,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (user == null) return RedirectToAction("Login");

            if (model.Username != user.Username)
            {
                bool usernameExists = await _context.Users.AnyAsync(u => u.Username == model.Username && u.Id != user.Id);
                if (usernameExists)
                {
                    ModelState.AddModelError("Username", "Tên tài khoản đã tồn tại, vui lòng chọn tên khác.");
                    return View(model);
                }
            }

            user.Username = model.Username;
            user.FullName = model.FullName;
            user.BirthDate = model.BirthDate;
            user.Gender = model.Gender;
            user.Email = model.Email;
            user.IdCard = model.IdCard;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;

            await _context.SaveChangesAsync();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["Success"] = "Cập nhật thông tin thành công";
            return RedirectToAction("Profile");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
