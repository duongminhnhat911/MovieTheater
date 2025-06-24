using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieManagementWeb_API.Models.Entities
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

        public string Cast { get; set; }

        public string Description { get; set; }

        public string ProductionCompany { get; set; }
        public List<string> Format { get; set; }

        public DateTime ReleaseDate { get; set; }
        public string CreatedByUsername { get; set; }
        public string EditedByUsername { get; set; }


        public ICollection<Showtime> Showtimes { get; set; }
        public ICollection<ActorMovie> ActorMovies { get; set; }
        public ICollection<MovieGenre> MovieGenres { get; set; }
    }
} 