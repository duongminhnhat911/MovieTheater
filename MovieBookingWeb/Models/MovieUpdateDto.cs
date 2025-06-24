using MovieBookingWeb.Models;

public class MovieUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Image { get; set; } = null!;
    public string CarouselImage { get; set; } = null!;
    public string TrailerLink { get; set; } = null!;
    public string Subtitle { get; set; } = null!;
    public string RatingCode { get; set; } = null!;
    public int Duration { get; set; }
    public List<string>? Genres { get; set; }
    public string Director { get; set; } = null!;
    public string Cast { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ProductionCompany { get; set; } = null!;
    public List<string>? Format { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string UpdatedByUsername { get; set; }
    public List<ShowtimeCreateDto>? Showtimes { get; set; }
}
