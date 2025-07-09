using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingManagement.Models.Entities

{
    public class OrderDetail
    {
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        [ForeignKey("Seat")]
        public int SeatId { get; set; }
        public Seat? Seat { get; set; }

        [ForeignKey("Schedule")]
        public int ShowtimeId { get; set; }
        public Showtime? Showtime { get; set; }

        public int Price { get; set; }
    }
}
