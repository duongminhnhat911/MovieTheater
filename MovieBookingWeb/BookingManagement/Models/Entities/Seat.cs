using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingManagement.Models.Entities
{
    public class Seat
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        public char SeatRow { get; set; }
        public char SeatColumn { get; set; }
        public bool SeatStatus { get; set; }

    }
}
