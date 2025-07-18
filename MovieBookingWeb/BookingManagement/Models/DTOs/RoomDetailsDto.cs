namespace BookingManagement.Models.DTOs
{
    public class RoomDetailsDto
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public int RoomQuantity { get; set; }
        public bool Status { get; set; }
        public List<string> Seats { get; set; } = new();
    }
}
