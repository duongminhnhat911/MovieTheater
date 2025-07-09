using System.ComponentModel.DataAnnotations;

namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class UpdateRoomDto
    {
        public int Id { get; set; }
        [Required]
        public string RoomName { get; set; }

        public bool Status { get; set; }

        [Range(1, 100)]
        public int Rows { get; set; }

        [Range(1, 100)]
        public int Columns { get; set; }
    }
}
