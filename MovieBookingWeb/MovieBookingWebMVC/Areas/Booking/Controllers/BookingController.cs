using Microsoft.AspNetCore.Mvc;
using MovieBookingWebMVC.Areas.Booking.Services;
using MovieBookingWebMVC.Areas.Movie.Services;
using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
using System.Net.Http;
using MovieBookingWebMVC.Areas.Booking.Models.ViewModels;
using Newtonsoft.Json;
using System.Text;
using System.Security.Claims;

namespace MovieBookingWebMVC.Areas.Booking.Controllers
{
    [Area("Booking")]
    public class BookingController : Controller
    {
        private readonly IBookingApiService _bookingApiService;
        private readonly MovieApiService _movieApiService;
        private readonly IShowtimeWebService _showtimeService;
        private readonly ILogger<BookingController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public BookingController(
            IBookingApiService bookingApiService,
            MovieApiService movieApiService,
            IShowtimeWebService showtimeService,
            IHttpClientFactory httpClientFactory,
            ILogger<BookingController> logger)
        {
            _bookingApiService = bookingApiService;
            _movieApiService = movieApiService;
            _showtimeService = showtimeService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Showtimes(int movieId)
        {
            var showtimes = await _showtimeService.GetShowtimesByMovieIdAsync(movieId);
            return View(showtimes);
        }

        public async Task<IActionResult> MoviesByDate(DateTime? date)
        {
            date ??= DateTime.Today;
            var client = _httpClientFactory.CreateClient();

            // Lấy tất cả suất chiếu
            var showtimeRes = await client.GetAsync("https://localhost:7116/api/Showtime");
            if (!showtimeRes.IsSuccessStatusCode)
                return View(new List<MovieWithShowtimeDTO>());

            var allShowtimes = await showtimeRes.Content.ReadFromJsonAsync<List<ShowtimeShortDTO>>();
            var selectedShowtimes = allShowtimes
                .Where(s => s.ShowDate.Date == date.Value.Date)
                .ToList();

            var groupedByMovie = selectedShowtimes.GroupBy(s => s.MovieId);
            var result = new List<MovieWithShowtimeDTO>();

            foreach (var group in groupedByMovie)
            {
                var movieId = group.Key;

                var movieRes = await client.GetAsync($"https://localhost:7197/api/Movies/{movieId}");
                if (!movieRes.IsSuccessStatusCode)
                    continue;

                var movie = await movieRes.Content.ReadFromJsonAsync<MovieWithShowtimeDTO>();
                movie.MovieId = movieId;
                movie.Showtimes = group.ToList();

                result.Add(movie);
            }

            ViewBag.SelectedDate = date.Value;

            // ✅ Thêm dòng này để truyền danh sách 7 ngày tới View
            ViewBag.WeekDates = Enumerable.Range(0, 7)
                .Select(offset => DateTime.Today.AddDays(offset))
                .ToList();

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> SelectSeat(int scheduleId)
        {
            _logger.LogInformation("SelectSeat started with scheduleId={ScheduleId}", scheduleId);

            // 1. Lấy lịch chiếu
            var schedule = await _bookingApiService.GetScheduleDetailAsync(scheduleId);
            if (schedule == null)
            {
                _logger.LogWarning("Không tìm thấy lịch chiếu với scheduleId={ScheduleId}", scheduleId);
                TempData["ErrorMessage"] = "Không tìm thấy lịch chiếu.";
                return RedirectToAction("MoviesByDate");
            }
            _logger.LogInformation("Lịch chiếu: Id={Id}, MovieId={MovieId}, RoomName={RoomName}, FromTime={FromTime}",
                schedule.Id, schedule.MovieId, schedule.RoomName ?? schedule.RoomName ?? "NULL", schedule.FromTime);

            // 2. Lấy danh sách ghế theo lịch chiếu
            var seatSchedule = await _bookingApiService.GetSeatSchedulesAsync(scheduleId);
            if (seatSchedule == null || seatSchedule.Seats == null)
            {
                _logger.LogWarning("Không lấy được thông tin ghế cho scheduleId={ScheduleId}", scheduleId);
                TempData["ErrorMessage"] = "Không lấy được thông tin ghế.";
                return RedirectToAction("Showtimes", new { movieId = schedule.MovieId });
            }
            _logger.LogInformation("Số lượng ghế lấy được: {SeatCount}", seatSchedule.Seats.Count);

            // 3. Lấy thông tin phim
            var movie = await _movieApiService.GetMovie(schedule.MovieId);
            if (movie == null)
            {
                _logger.LogWarning("Không tìm thấy thông tin phim với MovieId={MovieId}", schedule.MovieId);
                TempData["ErrorMessage"] = "Không tìm thấy thông tin phim.";
                return RedirectToAction("Showtimes", new { movieId = schedule.MovieId });
            }
            _logger.LogInformation("Thông tin phim: Title={Title}, Subtitle={Subtitle}", movie.Title, movie.Subtitle);

            // 4. Tạo ViewModel và trả về View
            var viewModel = new SeatBookingViewModel
            {
                ShowtimeId = schedule.Id,
                ShowDate = schedule.ShowDate,
                FromTime = schedule.FromTime,
                RoomName = schedule.RoomName,
                Movie = movie,
                Seats = seatSchedule.Seats
            };

            _logger.LogInformation("ViewModel tạo thành công. Returning view với {SeatCount} ghế", viewModel.Seats.Count);

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient_Booking");
            _logger.LogInformation("ConfirmOrder GET started for OrderId: {OrderId}", orderId);

            // 1. Lấy thông tin đơn hàng
            var orderRes = await client.GetAsync($"api/Order/{orderId}");
            if (!orderRes.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch Order with ID {OrderId}. StatusCode: {StatusCode}", orderId, orderRes.StatusCode);
                TempData["ErrorMessage"] = "Không lấy được đơn hàng.";
                return RedirectToAction("MoviesByDate");
            }

            var order = await orderRes.Content.ReadFromJsonAsync<OrderDTO>();
            _logger.LogInformation("Order fetched: UserId={UserId}, TotalPrice={TotalPrice}, Status={Status}",
                order.UserId, order.TotalPrice, order.Status);

            // 2. Lấy toàn bộ OrderDetail
            var detailsRes = await client.GetAsync("api/OrderDetail");
            if (!detailsRes.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch OrderDetail. StatusCode: {StatusCode}", detailsRes.StatusCode);
                TempData["ErrorMessage"] = "Không lấy được chi tiết đơn hàng.";
                return RedirectToAction("MoviesByDate");
            }

            var allDetails = await detailsRes.Content.ReadFromJsonAsync<List<OrderDetailDTO>>();
            var orderDetails = allDetails.Where(d => d.Order.Id == orderId).ToList();

            if (!orderDetails.Any())
            {
                _logger.LogWarning("No OrderDetail found for OrderId: {OrderId}", orderId);
                TempData["ErrorMessage"] = "Đơn hàng không có chi tiết nào.";
                return RedirectToAction("MoviesByDate");
            }

            _logger.LogInformation("Danh sách ghế đã chọn cho OrderId={OrderId}:", orderId);
            foreach (var d in orderDetails)
            {
                var s = d.Seat;
                _logger.LogInformation("- SeatId={SeatId}, Row={Row}, Column={Column}, Status={Status}",
                    s.SeatId, s.Row, s.Column, s.Status);
            }

            var detail = orderDetails.First();
            _logger.LogInformation("First detail: ShowtimeId={ShowtimeId}, RoomName={RoomName}",
                detail.Showtime.Id, detail.Showtime.RoomName ?? "NULL");

            var movie = await _movieApiService.GetMovie(detail.Showtime.MovieId);
            _logger.LogInformation("Movie fetched: {Title} - {Subtitle}", movie.Title, movie.Subtitle);

            var seatNames = orderDetails.Select(d => $"{d.Seat.Row}{d.Seat.Column}").ToList();
            _logger.LogInformation("Ghế đã chọn: {SeatList}", string.Join(", ", seatNames));

            var viewModel = new ConfirmBookingViewModel
            {
                OrderId = order.Id,
                UserId = order.UserId,
                BookingDate = order.BookingDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                Movie = new MovieDTO
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Subtitle = movie.Subtitle,
                    Image = movie.Image
                },
                Showtime = new ShowtimeDto
                {
                    Id = detail.Showtime.Id,
                    Room = detail.Showtime.Room,
                    ShowDate = detail.Showtime.ShowDate,
                    FromTime = detail.Showtime.FromTime
                },
                SelectedSeats = orderDetails.Select(d => d.Seat).ToList()
            };

            _logger.LogInformation("Seats in ViewModel: {Seats}",
                string.Join(", ", orderDetails.Select(d => $"{d.Seat?.Row}{d.Seat?.Column}")));
            _logger.LogInformation("Returning ConfirmBookingViewModel with {SeatCount} seats", viewModel.SelectedSeats.Count);

            return View("ConfirmOrder", viewModel);
        }

        [HttpPost]
        [ActionName("ConfirmOrder")]
        public async Task<IActionResult> ConfirmOrderPost(int orderId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient_Booking"); // ✅ SỬA: _ thay vì *
            _logger.LogInformation("ConfirmOrder POST called with OrderId={OrderId}", orderId);

            // Gọi API tạo URL thanh toán VnPay
            var response = await client.GetAsync($"api/Vnpay/create-url?orderId={orderId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Không thể tạo URL thanh toán.";
                return RedirectToAction("ConfirmOrder", new { orderId });
            }

            // ✅ SỬA: Đọc JSON response đúng cách
            var jsonString = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<dynamic>(jsonString);
            var paymentUrl = responseJson.paymentUrl?.ToString();

            if (string.IsNullOrEmpty(paymentUrl))
            {
                TempData["ErrorMessage"] = "Không thể tạo URL thanh toán.";
                return RedirectToAction("ConfirmOrder", new { orderId });
            }

            _logger.LogInformation("Redirecting to VnPay URL for OrderId: {OrderId}", orderId);
            return Redirect(paymentUrl);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(int showtimeId, List<int> seatIds)
        {
            if (seatIds == null || !seatIds.Any())
            {
                TempData["ErrorMessage"] = "Bạn chưa chọn ghế.";
                return RedirectToAction("SelectSeat", new { scheduleId = showtimeId });
            }

            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập trước khi đặt vé.";
                return RedirectToAction("Login", "Account", new { area = "User" });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                TempData["ErrorMessage"] = "Không thể xác định người dùng.";
                return RedirectToAction("Login", "Account", new { area = "User" });
            }// Hardcoded (nên thay bằng lấy từ User.Identity)
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            var request = new CreatePaymentRequestDto
            {
                ShowtimeId = showtimeId,
                UserId = userId,
                SeatIds = seatIds
            };

            var client = _httpClientFactory.CreateClient("ApiClient_Booking");

            // Gọi API tạo đơn hàng
            var response = await client.PostAsJsonAsync("api/Vnpay/create-url", request);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Lỗi khi tạo đơn hàng.";
                return RedirectToAction("SelectSeat", new { scheduleId = showtimeId });
            }

            var dto = await response.Content.ReadFromJsonAsync<OrderConfirmationDTO>();

            if (dto == null)
            {
                TempData["ErrorMessage"] = "Lỗi đọc dữ liệu đơn hàng.";
                return RedirectToAction("SelectSeat", new { scheduleId = showtimeId });
            }

            // Gọi API tạo URL thanh toán VnPay
            var vnpayUrlResponse = await client.GetAsync($"api/Vnpay/create-url?orderId={dto.OrderId}");

            if (!vnpayUrlResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Lỗi khi tạo URL thanh toán.";
                return RedirectToAction("ConfirmOrder", new { orderId = dto.OrderId });
            }

            // ✅ SỬA: Đọc JSON thay vì string thuần
            var responseJson = await vnpayUrlResponse.Content.ReadFromJsonAsync<dynamic>();
            var paymentUrl = responseJson.paymentUrl.ToString();

            // Chuyển hướng tới VnPay
            return Redirect(paymentUrl);
        }

        [HttpGet]
        public async Task<IActionResult> VNPayReturn()
        {
            try
            {
                _logger.LogInformation("🔄 VNPay return callback received");

                // Log tất cả parameters VNPay trả về
                _logger.LogInformation("📋 ALL VNPAY PARAMETERS:");
                foreach (var param in Request.Query)
                {
                    _logger.LogInformation("  {Key} = {Value}", param.Key, param.Value);
                }

                // Lấy các thông tin cần thiết từ VNPay response
                var responseCode = Request.Query["vnp_ResponseCode"].ToString();
                var transactionStatus = Request.Query["vnp_TransactionStatus"].ToString();
                var orderId = Request.Query["vnp_TxnRef"].ToString();
                var amount = Request.Query["vnp_Amount"].ToString();
                var transactionNo = Request.Query["vnp_TransactionNo"].ToString();

                // Kiểm tra kết quả thanh toán
                if (responseCode == "00" && transactionStatus == "00")
                {
                    _logger.LogInformation("✅ Payment SUCCESS! Processing order: {OrderId}", orderId);

                    // Cập nhật trạng thái đơn hàng thành công
                    var client = _httpClientFactory.CreateClient("ApiClient_Booking");
                    var updateResponse = await client.PostAsync($"api/Vnpay/confirm-payment?orderId={orderId}&transactionNo={transactionNo}", null);

                    if (updateResponse.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("✅ Order status updated successfully");
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Failed to update order status, but payment was successful");
                    }

                    // Set success message
                    TempData["SuccessMessage"] = "Thanh toán VNPay thành công! Đặt vé hoàn tất.";
                    TempData["BookingCode"] = $"BK{orderId.PadLeft(6, '0')}";
                    TempData["TransactionNo"] = transactionNo;
                    TempData["VNPaySuccess"] = true;

                    // ✅ SỬA: Redirect đến trang Booked thay vì ConfirmOrder
                    return RedirectToAction("Booked", new { orderId = orderId });
                }
                else
                {
                    _logger.LogWarning("❌ Payment FAILED!");
                    _logger.LogWarning("  Response Code: {ResponseCode}", responseCode);
                    _logger.LogWarning("  Transaction Status: {TransactionStatus}", transactionStatus);

                    TempData["ErrorMessage"] = "Thanh toán không thành công.";
                    return RedirectToAction("MoviesByDate");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Exception in VNPayReturn");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xử lý kết quả thanh toán.";
                return RedirectToAction("MoviesByDate");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SelectSeat(int showtimeId, List<int> seatIds)
        {
            if (seatIds == null || !seatIds.Any())
            {
                TempData["ErrorMessage"] = "Bạn chưa chọn ghế.";
                return RedirectToAction("SelectSeat", new { scheduleId = showtimeId });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                TempData["ErrorMessage"] = "Không thể xác định người dùng.";
                return RedirectToAction("Login", "Account", new { area = "User" });
            }
            var username = User.FindFirst(ClaimTypes.Name)?.Value;


            var request = new CreatePaymentRequestDto
            {
                ShowtimeId = showtimeId,
                UserId = userId,
                SeatIds = seatIds
            };

            var client = _httpClientFactory.CreateClient("ApiClient_Booking");
            var response = await client.PostAsJsonAsync("api/Order/payment", request);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Lỗi khi tạo đơn hàng.";
                return RedirectToAction("SelectSeat", new { scheduleId = showtimeId });
            }

            var dto = await response.Content.ReadFromJsonAsync<OrderConfirmationDTO>();
            if (dto == null)
            {
                TempData["ErrorMessage"] = "Không đọc được dữ liệu đơn hàng.";
                return RedirectToAction("SelectSeat", new { scheduleId = showtimeId });
            }

            return RedirectToAction("ConfirmOrder", new { orderId = dto.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> Booked(int orderId)
        {
            _logger.LogInformation("Booked page accessed for OrderId: {OrderId}", orderId);

            var viewModel = await _bookingApiService.MarkOrderAsBookedAsync(orderId);
            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Không thể xử lý đơn hàng.";
                return RedirectToAction("MoviesByDate");
            }

            return View("Booked", viewModel);
        }
    }
}