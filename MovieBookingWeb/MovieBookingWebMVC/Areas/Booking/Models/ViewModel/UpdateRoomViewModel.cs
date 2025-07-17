using System.ComponentModel.DataAnnotations;

namespace MovieBookingWebMVC.Areas.Booking.Models.ViewModel
{
    public class UpdateRoomViewModel
    {
        public int Id { get; set; }

        [Required]
        public string RoomName { get; set; }

        public bool Status { get; set; }

        public int Rows { get; set; }     // giữ giá trị gốc
        public int Columns { get; set; }  // giữ giá trị gốc
    }
}
