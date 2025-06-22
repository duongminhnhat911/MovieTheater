namespace MovieManagementWeb_API.Models.Entities
{
    public class MovieGenre
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }
} 