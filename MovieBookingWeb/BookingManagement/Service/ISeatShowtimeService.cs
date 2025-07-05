using BookingManagement.Models.Entities.Enums;
using BookingManagement.Models.Entities;
using BookingManagement.Models.DTOs;

namespace BookingManagement.Service
{
    public interface ISeatShowtimeService
    {
        Task<List<SeatShowtime>> GetAllAsync();
        Task<SeatShowtime?> GetAsync(int showtimeId, int seatId);
        Task<SeatShowtime> CreateAsync(SeatShowtime seatShowtime);
        Task<SeatShowtime?> UpdateStatusAsync(int showtimeId, int seatId, SeatStatus status);
        Task<bool> DeleteAsync(int showtimeId, int seatId);
        Task<SeatShowtimeDto?> GetSeatsByShowtimeAsync(int showtimeId);
    }
}
