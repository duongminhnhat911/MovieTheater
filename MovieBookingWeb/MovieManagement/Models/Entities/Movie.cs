using System.ComponentModel.DataAnnotations;

namespace MovieManagement.Models.Entities
{
    public class Movie
    {
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Title { get; set; }

    [StringLength(500)]
    public string Image { get; set; }

    [StringLength(500)]
    public string CarouselImage { get; set; }

    [StringLength(500)]
    public string TrailerLink { get; set; }

    public string Subtitle { get; set; }
    public string RatingCode { get; set; }
    public int? Duration { get; set; }

    public string Director { get; set; }
    public string Description { get; set; }

    public string ProductionCompany { get; set; }
    public List<string> Format { get; set; }

    public DateTime ReleaseDate { get; set; }
    public ICollection<ActorMovie> ActorMovies { get; set; }
    public ICollection<MovieGenre> MovieGenres { get; set; }

    // Status for soft delete: "Active", "SoldOut", "Deleted"
    public string Status { get; set; }

    // Track who created and last edited the movie
    public string? CreatedByUsername { get; set; }
    public string? EditedByUsername { get; set; }
    }
} 