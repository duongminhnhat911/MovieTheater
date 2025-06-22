using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieManagementWeb_API.Models.Entities
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public ICollection<ActorMovie> ActorMovies { get; set; }
    }
} 