namespace BookingManagement.Models.DTOs
{
    public class EditShowtimeDto
    {
        public int? RoomId { get; set; }
        public DateOnly? ShowDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public int BufferMinutes { get; set; } = 15;
    }
}
