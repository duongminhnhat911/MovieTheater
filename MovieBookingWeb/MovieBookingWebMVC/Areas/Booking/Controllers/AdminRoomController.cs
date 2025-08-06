using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
using MovieBookingWebMVC.Areas.Booking.Models.ViewModel;
using MovieBookingWebMVC.Areas.Booking.Services;
using System.Drawing.Printing;

namespace MovieBookingWebMVC.Areas.Booking.Controllers
{
    [Area("Booking")]
    [Authorize(Roles = "Admin")]
    public class AdminRoomController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<AdminRoomController> _logger;
        private const int PageSize = 5;

        public AdminRoomController(IRoomService roomService, ILogger<AdminRoomController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        // GET: /AdminRoom
        [Area("Booking")]
        [Route("Booking/AdminRoom")]
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var rooms = await _roomService.GetRoomsAsync();
                int totalRooms = rooms.Count;
                int totalPages = (int)Math.Ceiling((double)totalRooms / PageSize);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                var pagedRooms = rooms
                    .OrderBy(r => r.Id)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                return View(pagedRooms);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi lấy danh sách phòng: {ex.Message}";
                return View(new List<AdminRoomController>());
            }
        }

        // GET: /AdminRoom/Details/5
        [HttpGet]
        public async Task<IActionResult> DetailsRoom(int id)
        {
            _logger.LogInformation("Bắt đầu gọi GetRoomDetailsAsync với ID = {RoomId}", id);

            var room = await _roomService.GetRoomDetailsAsync(id);

            if (room == null)
            {
                _logger.LogWarning("Không tìm thấy phòng có ID = {RoomId}", id);
                return NotFound();
            }

            // Log chi tiết dữ liệu trả về
            _logger.LogInformation("Thông tin phòng: ID={Id}, Name={Name}, Quantity={Qty}, Status={Status}",
                room.Id, room.RoomName, room.RoomQuantity, room.Status);

            _logger.LogInformation("Danh sách ghế: {Seats}", string.Join(", ", room.Seats ?? new List<string>()));

#if DEBUG
            System.Diagnostics.Debug.WriteLine("========== DEBUG ROOM ==========");
            System.Diagnostics.Debug.WriteLine($"Room ID: {room.Id}");
            System.Diagnostics.Debug.WriteLine($"Name: {room.RoomName}");
            System.Diagnostics.Debug.WriteLine($"Quantity: {room.RoomQuantity}");
            System.Diagnostics.Debug.WriteLine($"Status: {(room.Status ? "Active" : "Inactive")}");
            System.Diagnostics.Debug.WriteLine("Seats:");
            if (room.Seats != null)
            {
                foreach (var seat in room.Seats)
                {
                    System.Diagnostics.Debug.WriteLine($" - {seat}");
                }
            }
            System.Diagnostics.Debug.WriteLine("================================");
#endif

            return View(room);
        }


        // GET: /AdminRoom/Create
        [HttpGet]
        public IActionResult CreateRoom()
        {
            return View();
        }

        // POST: /AdminRoom/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoom(CreateRoomDto dto)
        {
            Console.WriteLine($"RoomName: {dto.RoomName}, Rows: {dto.Rows}, Columns: {dto.Columns}, Status: {dto.Status}");

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var success = await _roomService.CreateRoomAsync(dto);
            if (success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Tạo phòng thất bại.");
            return View(dto);
        }
        //GET/Edit
        [HttpGet]
        public async Task<IActionResult> EditRoom(int id)
        {
            _logger.LogInformation("🔍===== [EditRoom - GET] =====");
            _logger.LogInformation("📡 [Step 1] Nhận yêu cầu GET EditRoom với id = {RoomId}", id);

            var vm = await _roomService.GetRoomViewModelForEditAsync(id);

            if (vm == null)
            {
                _logger.LogWarning("❌ [Step 2] Không lấy được ViewModel từ service với id = {RoomId}", id);
                return NotFound();
            }

            _logger.LogInformation("✅ [Step 2] ViewModel nhận được thành công:");
            _logger.LogInformation("🆔 Id: {Id}", vm.Id);
            _logger.LogInformation("🏷️ RoomName: {Name}", vm.RoomName);
            _logger.LogInformation("📏 Rows: {Rows}", vm.Rows);
            _logger.LogInformation("📐 Columns: {Columns}", vm.Columns);
            _logger.LogInformation("📌 Trạng thái: {Status}", vm.Status ? "Đang hoạt động" : "Tạm ngưng");

            if (vm.Id <= 0)
                _logger.LogWarning("⚠️ ID <= 0, có thể là giá trị không hợp lệ.");

            if (vm.Rows <= 0 || vm.Columns <= 0)
                _logger.LogWarning("⚠️ Rows hoặc Columns không hợp lệ. Có thể thiếu dữ liệu từ API.");

            _logger.LogInformation("🧭 Chuyển sang View EditRoom.");
            _logger.LogInformation("🟢===========================");

            return View(vm);
        }

        // POST: /AdminRoom/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoom(UpdateRoomViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _roomService.UpdateRoomFromViewModelAsync(model);
            if (success)
            {
                TempData["Success"] = "Cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Cập nhật phòng thất bại.");
            return View(model);
        }
    }
}
