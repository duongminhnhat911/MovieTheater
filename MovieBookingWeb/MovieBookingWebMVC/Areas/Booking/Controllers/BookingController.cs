using Microsoft.AspNetCore.Mvc;
using MovieBookingWebMVC.Areas.Booking.Services;
using MovieBookingWebMVC.Areas.Movie.Services;
using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
using System.Net.Http;
using MovieBookingWebMVC.Areas.Booking.Models.ViewModels;

namespace MovieBookingWebMVC.Areas.Booking.Controllers
{
    [Area("Booking")]
    public class BookingController : Controller
    {
        private readonly IBookingApiService _bookingApiService;
        private readonly MovieApiService _movieApiService;
        private readonly IShowtimeWebService _showtimeService;
        private readonly IHttpClientFactory _httpClientFactory;


        public BookingController(
            IBookingApiService bookingApiService,
            MovieApiService movieApiService,
            IShowtimeWebService showtimeService,
            IHttpClientFactory httpClientFactory)
        {
            _bookingApiService = bookingApiService;
            _movieApiService = movieApiService;
            _showtimeService = showtimeService;
            _httpClientFactory = httpClientFactory;
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
            var schedule = await _bookingApiService.GetScheduleDetailAsync(scheduleId);
            if (schedule == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch chiếu.";
                return RedirectToAction("MoviesByDate");
            }

            var seatSchedule = await _bookingApiService.GetSeatSchedulesAsync(scheduleId);
            if (seatSchedule == null)
            {
                TempData["ErrorMessage"] = "Không lấy được thông tin ghế.";
                return RedirectToAction("Showtimes", new { movieId = schedule.MovieId });
            }

            var movie = await _movieApiService.GetMovie(schedule.MovieId);
            if (movie == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin phim.";
                return RedirectToAction("Showtimes", new { movieId = schedule.MovieId });
            }

            var viewModel = new SeatBookingViewModel
            {
                ShowtimeId = schedule.Id,
                ShowDate = schedule.ShowDate,
                FromTime = schedule.FromTime,
                RoomName = schedule.RoomName,
                Movie = movie,
                Seats = seatSchedule.Seats
            };

            return View(viewModel);
        }

        //[HttpPost]
        //public async Task<IActionResult> ConfirmBooking(int showtimeId, List<int> seatIds)
        //{
        //    if (seatIds == null || !seatIds.Any())
        //    {
        //        TempData["ErrorMessage"] = "Bạn chưa chọn ghế.";
        //        return RedirectToAction("SelectSeat", new { scheduleId = showtimeId });
        //    }

        //    var userId = 2; // Hardcoded

        //    var request = new CreatePaymentRequestDto
        //    {
        //        ShowtimeId = showtimeId,
        //        UserId = userId,
        //        SeatIds = seatIds
        //    };

        //    var client = _httpClientFactory.CreateClient("ApiClient_Booking");
        //    var response = await client.PostAsJsonAsync("api/Order/payment", request);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        TempData["ErrorMessage"] = "Lỗi khi tạo đơn hàng.";
        //        return RedirectToAction("SelectSeat", new { scheduleId = showtimeId });
        //    }

        //    var dto = await response.Content.ReadFromJsonAsync<OrderConfirmationDTO>();
        //    if (dto == null)
        //    {
        //        TempData["ErrorMessage"] = "Lỗi đọc dữ liệu đơn hàng.";
        //        return RedirectToAction("SelectSeat", new { scheduleId = showtimeId });
        //    }

        //    return RedirectToAction("ConfirmOrder", new { orderId = dto.OrderId });
        //}

        [HttpGet]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            var client = _httpClientFactory.CreateClient("ApiClient_Booking");

            // 1. Lấy thông tin đơn hàng
            var orderRes = await client.GetAsync($"api/Order/{orderId}");
            if (!orderRes.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Không lấy được đơn hàng.";
                return RedirectToAction("MoviesByDate");
            }
            var order = await orderRes.Content.ReadFromJsonAsync<OrderDTO>();

            // 2. Lấy toàn bộ OrderDetail
            var detailsRes = await client.GetAsync("api/OrderDetail");
            if (!detailsRes.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Không lấy được chi tiết đơn hàng.";
                return RedirectToAction("MoviesByDate");
            }
            var allDetails = await detailsRes.Content.ReadFromJsonAsync<List<OrderDetailDTO>>();
            var orderDetails = allDetails.Where(d => d.Order.Id == orderId).ToList();

            if (!orderDetails.Any())
            {
                TempData["ErrorMessage"] = "Đơn hàng không có chi tiết nào.";
                return RedirectToAction("MoviesByDate");
            }

            var detail = orderDetails.First();
            var movie = await _movieApiService.GetMovie(detail.Showtime.MovieId);

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
                Showtime = new ShowtimeDTO
                {
                    Id = detail.Showtime.Id,
                    RoomName = detail.Showtime.RoomName,
                    ShowDate = detail.Showtime.ShowDate,
                    FromTime = detail.Showtime.FromTime
                },
                SelectedSeats = orderDetails.Select(d => d.Seat).ToList()
            };

            return View("ConfirmOrder", viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> SelectSeat(int showtimeId, List<int> seatIds)
        {
            if (seatIds == null || !seatIds.Any())
            {
                TempData["ErrorMessage"] = "Bạn chưa chọn ghế.";
                return RedirectToAction("SelectSeat", new { scheduleId = showtimeId });
            }

            var userId = 2; // Giả lập user

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
    }

}
