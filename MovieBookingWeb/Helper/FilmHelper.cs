using MovieBookingWeb.Models;
using System.Text.Json;
namespace MovieBookingWeb.Helper
{
    public static class FilmHelper
    {
        public static class RatingHelper
        {
            private static readonly Dictionary<string, string> _ratings = new()
            {
                { "P", "P - Phim được phép phổ biến đến người xem ở mọi độ tuổi." },
                { "K", "K - Phim được phổ biến đến người xem dưới 13 tuổi và có người bảo hộ đi kèm." },
                { "T13", "T13 - Phim được phổ biến đến người xem từ đủ 13 tuổi trở lên (13+)." },
                { "T16", "T16 - Phim được phổ biến đến người xem từ đủ 16 tuổi trở lên (16+)." },
                { "T18", "T18 - Phim được phổ biến đến người xem từ đủ 18 tuổi trở lên (18+)." }
            };
            public static Dictionary<string, string> GetRatings() => _ratings;
            public static string? GetFullRating(string code)
            {
                return _ratings.TryGetValue(code, out var value) ? value : null;
            }
            public static bool IsValidCode(string code) => _ratings.ContainsKey(code);
        }
        public static class ShowtimeUtils
        {
            public static Dictionary<string, Dictionary<string, string>> GenerateRoomByDateTime(List<Showtime> showtimes)
            {
                return showtimes
                    .Where(s => !string.IsNullOrEmpty(s.RoomName))
                    .GroupBy(s => s.Date.ToString("yyyy-MM-dd"))
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToDictionary(
                            s => s.Time.ToString(@"hh\:mm"),
                            s => s.RoomName!
                        )
                    );
            }
        }
        public static string ConvertShowtimesToJson(List<Showtime> showtimes)
        {
            var simplified = showtimes.Select(s => new
            {
                date = s.Date.ToString("yyyy-MM-dd"),
                time = s.Time.ToString(@"hh\:mm"),
                roomName = s.RoomName
            });
            return JsonSerializer.Serialize(simplified);
        }
        public static List<Showtime> ParseShowtimesFromJson(string? json)
        {
            var showtimes = new List<Showtime>();
            if (string.IsNullOrWhiteSpace(json)) return showtimes;
            try
            {
                var elements = JsonSerializer.Deserialize<List<JsonElement>>(json);
                foreach (var el in elements ?? [])
                {
                    var dateStr = el.GetProperty("date").GetString();
                    var timeStr = el.GetProperty("time").GetString();
                    var roomName = el.GetProperty("roomName").GetString();

                    showtimes.Add(new Showtime
                    {
                        Date = DateTime.TryParse(dateStr, out var date) ? date : default,
                        Time = TimeSpan.TryParse(timeStr, out var time) ? time : default,
                        RoomName = roomName ?? ""
                    });
                }
            }
            catch { }
            return showtimes;
        }
        public static async Task SaveImagesAsync(Film film, string uploadPath, bool useGuidName = true)
        {
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            if (film.ImageFile != null && film.ImageFile.Length > 0)
            {
                var fileName = useGuidName ? Guid.NewGuid() + Path.GetExtension(film.ImageFile.FileName)
                                           : Path.GetFileName(film.ImageFile.FileName);
                var filePath = Path.Combine(uploadPath, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await film.ImageFile.CopyToAsync(stream);
                film.Image = "/uploads/" + fileName;
            }

            if (film.CarouselImageFile != null && film.CarouselImageFile.Length > 0)
            {
                var fileName = useGuidName ? Guid.NewGuid() + Path.GetExtension(film.CarouselImageFile.FileName)
                                           : Path.GetFileName(film.CarouselImageFile.FileName);
                var filePath = Path.Combine(uploadPath, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await film.CarouselImageFile.CopyToAsync(stream);
                film.CarouselImage = "/uploads/" + fileName;
            }
        }
    }
}
