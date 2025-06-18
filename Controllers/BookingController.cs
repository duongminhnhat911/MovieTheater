using Microsoft.AspNetCore.Mvc;
using MovieBookingWeb.Models;
using MovieBookingWeb.Services;

namespace MovieBookingWeb.Controllers
{
    public class BookingController : Controller
    {
        private readonly List<Film> films = new List<Film>
        {
            new Film {
                Title = "Dune: Hành tinh cát",
                Image = "/images/dune.jpg",
                BookingLink = "/booking/dune",
                Genre = "Khoa học viễn tưởng",
                Duration = "155 phút",
                Rating = "T16 - Phim được phổ biến đến người xem từ đủ 16 tuổi trở lên (16+)",
                Subtitle = "Tiếng Anh - Phụ đề Tiếng Việt",
                Director = "Denis Villeneuve",
                Cast = "Timothée Chalamet, Rebecca Ferguson, Oscar Isaac, Josh Brolin, Stellan Skarsgård, Dave Bautista, Stephen M. Henderson, Zendaya, Javier Bardem, Jason Momoa...",
                ReleaseDate = new DateTime(2021, 10, 21),
                Description = "Dune lấy bối cảnh tương lai xa của loài người, một vị công tước mang tên Leto Atreides đang quản lý hành tinh sa mạc nguy hiểm Arrakis – nơi sở hữu nguồn vật chất quý giá nhất trong vũ trụ có thể giúp con người kéo dài sự sống, sở hữu năng lực siêu phàm và có khả năng du hành không gian. Mặc dù biết rằng có thể bị kẻ thù là nhà Harkonnens gài bẫy, nhưng Leto Atreides vẫn quyết tâm lấy công nương Jessica làm vợ, và nuôi nấng đứa con trai của cô - Paul Atreides, thường được mọi người gọi là Dune. Về sau Jessica và Paul bị phản bội, buộc phải lưu lạc đến Fremen, nơi sa mạc vô cùng hoang sơ có những người Arrakis sinh sống và bắt đầu một hành trình mới đầy trắc trở…",
                Showtimes = new List<Showtime>
                {
                    new Showtime { Date = DateTime.Parse("2021-10-21"), Time = "10:00", RoomName = "Rạp 3" },
                    new Showtime { Date = DateTime.Parse("2021-10-21"), Time = "14:00", RoomName = "Rạp 2" },
                    new Showtime { Date = DateTime.Parse("2021-10-21"), Time = "18:00", RoomName = "Rạp 4" },
                    new Showtime { Date = DateTime.Parse("2021-10-22"), Time = "12:00", RoomName = "Rạp 3" },
                    new Showtime { Date = DateTime.Parse("2021-10-22"), Time = "16:00", RoomName = "Rạp 1" },
                    new Showtime { Date = DateTime.Parse("2021-10-23"), Time = "20:00", RoomName = "Rạp 5" },
                    new Showtime { Date = DateTime.Parse("2021-10-23"), Time = "18:00", RoomName = "Rạp 2" }
                },
                RoomByDateTime = new Dictionary<string, Dictionary<string, string>>
                {
                    { "2021-10-21", new Dictionary<string, string> {
                        { "10:00", "Rạp 3" }, { "14:00", "Rạp 2" }, { "18:00", "Rạp 4" }
                    }},
                    { "2021-10-22", new Dictionary<string, string> {
                        { "12:00", "Rạp 3" }, { "16:00", "Rạp 1" }, { "20:00", "Rạp 5" }
                    }},
                    { "2021-10-23", new Dictionary<string, string> {
                        { "18:00", "Rạp 2" }
                    }}
                },
                BookedSeats = new Dictionary<string, Dictionary<string, List<string>>>
                {
                    { "2021-10-21", new Dictionary<string, List<string>> {
                        { "10:00", new List<string> { "A1", "A2", "B5" } },
                        { "14:00", new List<string> { "C3", "C4" } }
                    }},
                    { "2021-10-22", new Dictionary<string, List<string>> {
                        { "12:00", new List<string> { "D1", "D2", "D3" } },
                        { "16:00", new List<string> { "A10", "A11" } }
                    }},
                    { "2021-10-23", new Dictionary<string, List<string>> {
                        { "18:00", new List<string> { "B1", "B2", "B3", "C1" } }
                    }}
                }
            },
            new Film {
                Title = "Inside Out 2",
                Image = "/images/insideout2.jpg",
                BookingLink = "/booking/inside-out-2",
                Genre = "Hài, Hoạt Hình, Phiêu Lưu",
                Duration = "96 phút",
                Rating = "P - Phim được phép phổ biến đến người xem ở mọi độ tuổi.",
                Subtitle = "Tiếng Anh - Phụ đề Tiếng Việt, Lồng tiếng Việt",
                Director = "Kelsey Mann",
                Cast = "(Lồng tiếng) Amy Poehler, Maya Hawke, Lewis Black, Phyllis Smith, Tony Hale, Liza Lapira, Ayo Edebiri, Adèle Exarchopoulos, Paul Walter Hauser,...",
                ReleaseDate = new DateTime(2024, 6, 14),
                Description = "Cuộc sống tuổi mới lớn của cô bé Riley lại tiếp tục trở nên hỗn loạn hơn bao giờ hết với sự xuất hiện của 4 cảm xúc hoàn toàn mới: Lo u, Ganh Tị, Xấu Hổ, Chán Nản. Mọi chuyện thậm chí còn rối rắm hơn khi nhóm cảm xúc mới này nổi loạn và nhốt nhóm cảm xúc cũ gồm Vui Vẻ, Buồn Bã, Giận Dữ, Sợ Hãi và Chán Ghét lại, khiến cô bé Riley rơi vào những tình huống dở khóc dở cười.",
                Showtimes = new List<Showtime>
                {
                    new Showtime { Date = DateTime.Parse("2024-06-14"), Time = "10:00", RoomName = "Rạp 1" },
                    new Showtime { Date = DateTime.Parse("2024-06-14"), Time = "14:00", RoomName = "Rạp 2" },
                    new Showtime { Date = DateTime.Parse("2024-06-15"), Time = "16:00", RoomName = "Rạp 3" }
                },
                RoomByDateTime = new Dictionary<string, Dictionary<string, string>>
                {
                    { "2024-06-14", new Dictionary<string, string> {
                        { "10:00", "Rạp 1" }, { "14:00", "Rạp 2" }
                    }},
                    { "2024-06-15", new Dictionary<string, string> {
                        { "16:00", "Rạp 3" }
                    }}
                },
                BookedSeats = new Dictionary<string, Dictionary<string, List<string>>>
                {
                    { "2024-06-14", new Dictionary<string, List<string>> {
                        { "10:00", new List<string> { "A1", "A2" } }
                    }}
                }
            },
            new Film {
                Title = "Godzilla x Kong",
                Image = "/images/godzilla-vs-kong.jpg",
                BookingLink = "/booking/godzilla-vs-kong",
            }
        };
        private readonly RoomLayoutService _roomLayoutService;
        public BookingController(RoomLayoutService roomLayoutService)
        {
            _roomLayoutService = roomLayoutService;
        }

        public IActionResult Index(string id)
        {
            var film = films.FirstOrDefault(f => f.BookingLink != null && f.BookingLink.EndsWith("/" + id));
            if (film == null)
            {
                return NotFound($"Không tìm thấy phim với id: {id}");
            }
            var viewModel = new BookingViewModel
            {
                Title = film.Title,
                Image = film.Image,
                Genre = film.Genre,
                Duration = film.Duration,
                Rating = film.Rating,
                Subtitle = film.Subtitle,
                Director = film.Director,
                Cast = film.Cast,
                ReleaseDate = film.ReleaseDate,
                Description = film.Description,
                Showtimes = film.Showtimes ?? new List<Showtime>(),
                RoomByDateTime = film.RoomByDateTime ?? new Dictionary<string, Dictionary<string, string>>(),
                BookedSeats = film.BookedSeats ?? new Dictionary<string, Dictionary<string, List<string>>>(),
                RoomLayouts = _roomLayoutService.GetAllRoomLayouts(),
                AvailableDates = (film.Showtimes ?? new List<Showtime>())
                    .Select(s => s.Date.ToString("yyyy-MM-dd"))
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Confirm(string SelectedSeats)
        {
            return Content($"Bạn đã đặt các ghế: {SelectedSeats}");
        } 
    }
}