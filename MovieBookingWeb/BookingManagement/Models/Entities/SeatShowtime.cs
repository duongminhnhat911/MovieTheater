using System.ComponentModel.DataAnnotations;
using BookingManagement.Models.Entities.Enums;

namespace BookingManagement.Models.Entities
{
    public class SeatShowtime
    {
        public int SeatId { get; set; }
        public Seat? Seat { get; set; }
        public int ShowtimeId { get; set; }
        public Showtime? Showtime { get; set; }

        public SeatStatus Status { get; set; } = SeatStatus.Available;

        //RealTime
        //public int? HeldByUserId { get; set; }
        //public DateTime? HeldUntil { get; set; }
    }
}
