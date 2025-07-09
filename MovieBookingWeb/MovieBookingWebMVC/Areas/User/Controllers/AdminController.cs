using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MovieBookingWebMVC.Areas.Movie.Services;
using MovieBookingWebMVC.Areas.User.Models.DTOs;

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
            public async Task<IActionResult> EditAccount(int id)
            {
                var client = _httpClientFactory.CreateClient("ApiClient_User");
                var response = await client.GetAsync($"api/user/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không tìm thấy người dùng.";
                    return RedirectToAction("ViewAccounts");
                }

                var json = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<AdminUpdateUserDto>(json);
                return View(user);
            }

            [Authorize(Roles = "Admin")]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> EditAccount(AdminUpdateUserDto dto)
            {
                if (!ModelState.IsValid) return View(dto);

                var client = _httpClientFactory.CreateClient("ApiClient_User");
                var response = await client.PutAsJsonAsync($"api/user/{dto.Id}", dto);

                TempData["Success"] = response.IsSuccessStatusCode
                    ? "Cập nhật người dùng thành công!"
                    : "Cập nhật thất bại.";
                return RedirectToAction("ViewAccounts");
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteAccount(int id)
            {
                var client = _httpClientFactory.CreateClient("ApiClient_User");

                var response = await client.DeleteAsync($"api/user/{id}");

                TempData["Success"] = response.IsSuccessStatusCode
                    ? "Đã thay đổi trạng thái khoá tài khoản."
                    : "Không thể thay đổi trạng thái khoá tài khoản.";

                return RedirectToAction("ViewAccounts");
            }
        }
    }
