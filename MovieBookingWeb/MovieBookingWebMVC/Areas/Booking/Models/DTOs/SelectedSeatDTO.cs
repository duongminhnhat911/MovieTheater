using System.Text.Json.Serialization;

namespace MovieBookingWebMVC.Areas.Booking.Models.DTOs
{
    public class SelectedSeatDTO
    {
        public int SeatId { get; set; }

        [JsonPropertyName("seatRow")]
        public string Row { get; set; } = string.Empty;

        [JsonPropertyName("seatColumn")]
        public string Column { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
