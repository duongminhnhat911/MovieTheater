using Microsoft.AspNetCore.Mvc;
using MovieBookingWeb.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _contextAccessor;

    public AccountController(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _contextAccessor = contextAccessor;
    }

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _httpClientFactory.CreateClient("ApiClient_User");
        var response = await client.PostAsJsonAsync("api/auth/login", model);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu");
            return View(model);
        }

        var userJson = await response.Content.ReadAsStringAsync();
        var user = JsonConvert.DeserializeObject<UserDto>(userJson);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return user.Role switch
        {
            "Admin" => RedirectToAction("ViewMovie", "Admin"),
            "Employee" => RedirectToAction("Dashboard", "Employee"),
            _ => RedirectToAction("Profile", "Account")
        };
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
}