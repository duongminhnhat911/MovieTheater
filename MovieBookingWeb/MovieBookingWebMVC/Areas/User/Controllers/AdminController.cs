using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBookingWebMVC.Areas.Movie.Services;
using MovieBookingWebMVC.Areas.User.Models.DTOs;
using Newtonsoft.Json;
using System.Reflection;

namespace MovieBookingWebMVC.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IRoomService _roomService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly MovieApiService _movieApiService;
        private const int PageSize = 5;

        public AdminController(
            IWebHostEnvironment environment,
            IRoomService roomService,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor contextAccessor,
            MovieApiService movieApiService)
        {
            _environment = environment;
            _roomService = roomService;
            _httpClientFactory = httpClientFactory;
            _contextAccessor = contextAccessor;
            _movieApiService = movieApiService;
        }

        private async Task<UserDto?> GetCurrentUserAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient_User");
            var cookie = _contextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
            if (!string.IsNullOrEmpty(cookie))
                client.DefaultRequestHeaders.Add("Cookie", cookie);

            var response = await client.GetAsync("api/user/profile");
            if (!response.IsSuccessStatusCode) return null;

            var userJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserDto>(userJson);
        }

        [HttpGet]
        public async Task<IActionResult> ViewAccounts(int page = 1)
        {
            var client = _httpClientFactory.CreateClient("ApiClient_User");
            var cookie = _contextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
            if (!string.IsNullOrEmpty(cookie))
                client.DefaultRequestHeaders.Add("Cookie", cookie);

            var response = await client.GetAsync("api/user");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("StatusCode: " + response.StatusCode);
                TempData["Error"] = "Lỗi khi tải danh sách người dùng.";
                return RedirectToAction("ViewMovie");
            }

            var json = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<GetListUserDto>>(json);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = 1;
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var client = _httpClientFactory.CreateClient("ApiClient_User");
            var cookie = _contextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
            if (!string.IsNullOrEmpty(cookie))
                client.DefaultRequestHeaders.Add("Cookie", cookie);

            var response = await client.GetAsync($"api/user/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction("ViewAccounts");
            }

            var json = await response.Content.ReadAsStringAsync();

            // DEBUG: In ra JSON thô để xem API trả về gì
            Console.WriteLine("=== RAW JSON FROM API ===");
            Console.WriteLine(json);
            Console.WriteLine("========================");

            // Thử deserialize và xem kết quả
            try
            {
                var user = JsonConvert.DeserializeObject<AdminUpdateUserDto>(json);

                Console.WriteLine("=== DESERIALIZED DATA ===");
                Console.WriteLine($"BirthDate: '{user?.BirthDate}' (is null: {user?.BirthDate == null})");
                Console.WriteLine($"Gender: '{user?.Gender}' (is null/empty: {string.IsNullOrEmpty(user?.Gender)})");
                Console.WriteLine($"IdCard: '{user?.IdCard}' (is null/empty: {string.IsNullOrEmpty(user?.IdCard)})");
                Console.WriteLine($"PhoneNumber: '{user?.PhoneNumber}' (is null/empty: {string.IsNullOrEmpty(user?.PhoneNumber)})");
                Console.WriteLine($"Address: '{user?.Address}' (is null/empty: {string.IsNullOrEmpty(user?.Address)})");
                Console.WriteLine("=========================");

                var editModel = new EditUserDto
                {
                    Id = user.Id,
                    Username = user.Username ?? string.Empty,
                    FullName = user.FullName ?? string.Empty,
                    BirthDate = user.BirthDate,
                    Gender = user.Gender ?? string.Empty,
                    IdCard = user.IdCard ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Address = user.Address ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Role = user.Role ?? string.Empty
                };

                return View(editModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DESERIALIZATION ERROR: {ex.Message}");
                TempData["Error"] = "Lỗi khi xử lý dữ liệu người dùng.";
                return RedirectToAction("ViewAccounts");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient_User");
                var cookie = _contextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
                if (!string.IsNullOrEmpty(cookie))
                    client.DefaultRequestHeaders.Add("Cookie", cookie);

                // Convert EditUserDto sang AdminUpdateUserDto nếu API yêu cầu
                var updateDto = new AdminUpdateUserDto
                {
                    Id = model.Id,
                    Username = model.Username,
                    FullName = model.FullName,
                    BirthDate = model.BirthDate,
                    Gender = model.Gender,
                    IdCard = model.IdCard,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Email = model.Email,
                    Role = model.Role
                };

                var response = await client.PutAsJsonAsync($"api/user/{model.Id}", updateDto);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Cập nhật thông tin người dùng thành công!";
                    return RedirectToAction("ViewAccounts");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Cập nhật thất bại: {errorContent}");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient_User");
                var cookie = _contextAccessor.HttpContext?.Request.Headers["Cookie"].ToString();
                if (!string.IsNullOrEmpty(cookie))
                    client.DefaultRequestHeaders.Add("Cookie", cookie);

                var response = await client.DeleteAsync($"api/user/{id}");

                TempData["Success"] = response.IsSuccessStatusCode
                    ? "Đã thay đổi trạng thái khóa tài khoản."
                    : "Không thể thay đổi trạng thái khóa tài khoản.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
            }

            return RedirectToAction("ViewAccounts");
        }
    }
}