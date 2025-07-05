namespace BookingManagement.Models.DTOs
{
    public class UpdateSeatDto
    {
        public bool? SeatStatus { get; set; } // true = active, false = blocked
    }
}
