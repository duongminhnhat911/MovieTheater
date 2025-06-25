using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieManagementWeb_API.Models.Entities
{
    public class Showtime
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string RoomName { get; set; }
        public int MovieId { get; set; }
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }
    }
} 