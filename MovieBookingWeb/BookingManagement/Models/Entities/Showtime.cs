using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingManagement.Models.Entities
{
    public class Showtime
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room? Room { get; set; }

        public int MovieId { get; set; }

        public required DateOnly ShowDate { get; set; }
        public required TimeOnly FromTime { get; set; }
        public required TimeOnly ToTime { get; set; }
    }
}
