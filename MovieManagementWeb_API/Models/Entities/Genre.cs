using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieManagementWeb_API.Models.Entities
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<MovieGenre> MovieGenres { get; set; }
    }
} 