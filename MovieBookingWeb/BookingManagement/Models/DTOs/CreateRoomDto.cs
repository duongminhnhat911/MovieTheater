namespace BookingManagement.Models.DTOs
{
    public class CreateRoomDto
    {
        public string RoomName { get; set; }
        public bool Status { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
    }
}
