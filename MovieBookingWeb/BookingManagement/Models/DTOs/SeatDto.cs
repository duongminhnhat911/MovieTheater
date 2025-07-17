namespace BookingManagement.Models.DTOs
{
    public class SeatDto
    {
        public int SeatId { get; set; }
        public char Row { get; set; }
        public char Column { get; set; }
        public string Status { get; set; } = "Available"; // Available / Held / Booked
    }
}
