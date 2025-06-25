public async Task<IActionResult> Index()
{
    try
    {
        var films = await _movieApiService.GetMovies();

        foreach (var film in films)
        {
            film.BookingLink = Url.Action("Index", "Booking", new { id = film.Id });
        }

        var carousel = films.Where(f => !string.IsNullOrEmpty(f.CarouselImage)).ToList();
        var nowShowing = films.Where(f => f.ReleaseDate <= DateTime.Now).ToList();
        var comingSoon = films.Where(f => f.ReleaseDate > DateTime.Now).ToList();

        var model = new HomeViewModel
        {
            Carousel = carousel,
            NowShowing = nowShowing,
            ComingSoon = comingSoon
        };
        return View(model);
    }
    catch (HttpRequestException ex)
    {
        ViewData["Error"] = "Không thể tải dữ liệu phim lúc này. Vui lòng thử lại sau.";
        return View(new HomeViewModel());
    }
}