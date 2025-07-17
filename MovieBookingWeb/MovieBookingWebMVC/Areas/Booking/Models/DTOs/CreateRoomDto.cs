using System.ComponentModel.DataAnnotations;

namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class CreateRoomDto
    {
            public string RoomName { get; set; }
            public int Rows { get; set; }
            public int Columns { get; set; }
            public bool Status { get; set; } // <-- quan trọng
        }
    }
