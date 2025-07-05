namespace BookingManagement.Models.DTOs
{
    public class SeatShowtimeDto
    {
        public int RoomId { get; set; }
        public int ShowtimeId { get; set; }
        public List<SeatDto> Seats { get; set; } = new();
    }
}
