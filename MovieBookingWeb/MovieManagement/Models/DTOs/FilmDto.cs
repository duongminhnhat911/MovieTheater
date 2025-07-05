namespace MovieManagement.Models.DTOs
{
    public class FilmDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Image { get; set; }
        public string? CarouselImage { get; set; }
        public string? TrailerLink { get; set; }
        public string? Subtitle { get; set; }
        public string? RatingCode { get; set; }
        public string? Rating { get; set; }
        public int? Duration { get; set; }

        public List<string>? Genres { get; set; }
        public List<string>? Actors { get; set; } // ✅ Thay thế Cast

        public string? Director { get; set; }
        public string? Description { get; set; }
        public string? ProductionCompany { get; set; }
        public List<string>? Format { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public string? CreatedByUsername { get; set; }
        public string? EditedByUsername { get; set; }
        public string? Status { get; set; }
    }
}
