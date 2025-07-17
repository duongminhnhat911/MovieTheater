namespace BookingManagement.Models.Entities.Enums
{
    public enum SeatStatus
    {
        Available = 0,  // Ghế trống, có thể chọn
        Held = 1,       // Đang được giữ tạm thời (5-10 phút)
        Booked = 2      // Đã đặt chính thức, không thể chọn
    }
}
