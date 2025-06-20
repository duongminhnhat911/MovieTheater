using MovieBookingWeb.Models;
using MovieBookingWeb.Helper;
public static class MockFilmData
{
    public static List<Film> Films { get; }
    static MockFilmData()
    {
        Films = new List<Film>
        {
            new Film {
                Id = 1,
                Title = "Dune: Hành tinh cát",
                Tagline = "Sử thi khoa học viễn tưởng của năm",
                ReleaseDate = new DateTime(2021, 10, 21),
                Image = "/images/dune.jpg",
                CarouselImage = "/images/dune-carousel.jpg",
                BookingLink = "/booking/dune",
                TrailerLink = "https://www.youtube.com/watch?v=R797Sf-2zt0",
                Genres = "Khoa học viễn tưởng".Split(",").Select(g => g.Trim()).ToList(),
                Duration = 155,
                RatingCode = "T16",
                Rating = FilmHelper.RatingHelper.GetFullRating("T16"),
                Format = "2D".Split(",").Select(f => f.Trim()).ToList(),
                Subtitle = "Tiếng Anh - Phụ đề Tiếng Việt",
                ProductionCompany = "Legendary Pictures",
                Director = "Denis Villeneuve",
                Cast = "Timothée Chalamet, Rebecca Ferguson, Oscar Isaac, Josh Brolin, Stellan Skarsgård, Zendaya...",
                Description = "Dune lấy bối cảnh tương lai xa của loài người, một vị công tước mang tên Leto Atreides đang quản lý hành tinh sa mạc nguy hiểm Arrakis.",
                Showtimes = new List<Showtime>
                {
                    new Showtime { Date = DateTime.Parse("2021-10-21"), Time = TimeSpan.Parse("10:00"), RoomName = "Rạp 3" },
                    new Showtime { Date = DateTime.Parse("2021-10-21"), Time = TimeSpan.Parse("14:00"), RoomName = "Rạp 2" },
                    new Showtime { Date = DateTime.Parse("2021-10-21"), Time = TimeSpan.Parse("18:00"), RoomName = "Rạp 4" },
                    new Showtime { Date = DateTime.Parse("2021-10-22"), Time = TimeSpan.Parse("12:00"), RoomName = "Rạp 3" },
                    new Showtime { Date = DateTime.Parse("2021-10-22"), Time = TimeSpan.Parse("16:00"), RoomName = "Rạp 1" },
                    new Showtime { Date = DateTime.Parse("2021-10-23"), Time = TimeSpan.Parse("20:00"), RoomName = "Rạp 5" },
                    new Showtime { Date = DateTime.Parse("2021-10-23"), Time = TimeSpan.Parse("18:00"), RoomName = "Rạp 2" }
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
                Id = 3,
                Title = "Inside Out 2",
                Tagline = "Hành trình cảm xúc tuổi teen",
                ReleaseDate = new DateTime(2024, 6, 14),
                Image = "/images/insideout2.jpg",
                CarouselImage = "/images/carousel-io2.jpg",
                BookingLink = "/booking/inside-out-2",
                TrailerLink = "https://www.youtube.com/watch?v=AfOlW2OrzqE",
                Genres = "Hài, Hoạt hình, Phiêu lưu".Split(",").Select(g => g.Trim()).ToList(),
                Duration = 96,
                RatingCode = "P",
                Rating = FilmHelper.RatingHelper.GetFullRating("P"),
                Format = "2D".Split(",").Select(f => f.Trim()).ToList(),
                Subtitle = "Tiếng Anh - Phụ đề Tiếng Việt, Lồng tiếng Việt",
                ProductionCompany = "Pixar Animation Studios",
                Director = "Kelsey Mann",
                Cast = "(Lồng tiếng) Amy Poehler, Maya Hawke, Lewis Black, Phyllis Smith, Tony Hale, Liza Lapira, Ayo Edebiri,...",
                Description = "Cuộc sống tuổi mới lớn của cô bé Riley lại tiếp tục trở nên hỗn loạn hơn bao giờ hết với sự xuất hiện của 4 cảm xúc hoàn toàn mới: Lo u, Ganh Tị, Xấu Hổ, Chán Nản.",
                Showtimes = new List<Showtime>
                {
                    new Showtime { Date = DateTime.Parse("2024-06-14"), Time = TimeSpan.Parse("10:00"), RoomName = "Rạp 1" },
                    new Showtime { Date = DateTime.Parse("2024-06-14"), Time = TimeSpan.Parse("14:00"), RoomName = "Rạp 2" },
                    new Showtime { Date = DateTime.Parse("2024-06-15"), Time = TimeSpan.Parse("18:00"), RoomName = "Rạp 3" }
                },
                BookedSeats = new Dictionary<string, Dictionary<string, List<string>>>
                {
                    { "2024-06-14", new Dictionary<string, List<string>> {
                        { "10:00", new List<string> { "A1", "A2" } }
                    }}
                }
            },
            new Film {
                Id = 2,
                Title = "Godzilla x Kong",
                Tagline = "Cuộc đối đầu thế kỷ",
                ReleaseDate = new DateTime(2024, 3, 29),
                Image = "/images/godzilla-vs-kong.jpg",
                CarouselImage = "/images/godzillavskong-carousel.jpg",
                TrailerLink = "/trailer/godzilla-vs-kong",
                BookingLink = "/booking/godzilla-vs-kong",
                Genres = "Hành động".Split(",").Select(g => g.Trim()).ToList(),
                Duration = 115,
                Showtimes = new List<Showtime>
                {
                    new Showtime { Date = DateTime.Parse("2025-06-20"), Time = TimeSpan.Parse("20:30"), RoomName = "Rạp 2" }
                }
            }
        };

        foreach (var film in Films)
        {
            film.AvailableDates = film.Showtimes!
                .Select(s => s.Date.ToString("yyyy-MM-dd"))
                .Distinct()
                .OrderBy(d => d)
                .ToList();
            film.RoomByDateTime = FilmHelper.ShowtimeUtils.GenerateRoomByDateTime(film.Showtimes!);
        }
    }

    public static void UpdateShowtimes(int filmId, List<Showtime> newShowtimes)
    {
        var film = Films.FirstOrDefault(f => f.Id == filmId);
        if (film == null) return;
        film.Showtimes = newShowtimes.OrderBy(s => s.Date).ThenBy(s => s.Time).ToList();
        film.AvailableDates = film.Showtimes.Select(s => s.Date.ToString("yyyy-MM-dd")).Distinct().ToList();
    }
}
