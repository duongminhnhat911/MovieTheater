using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
using BookingManagement.Service;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Repositories
{
    // Repositories/ShowtimeRepository.cs
    public class ShowtimeRepository : IShowtimeRepository
    {
        private readonly BookingDbContext _db;
        private readonly IMovieServiceClient _movieService;

        public ShowtimeRepository(BookingDbContext db, IMovieServiceClient movieService)
        {
            _db = db;
            _movieService = movieService;
        }

        public Task<Room?> GetActiveRoomAsync(int roomId) =>
            _db.Rooms.FirstOrDefaultAsync(r => r.Id == roomId && r.Status);

        public Task<List<Seat>> GetSeatsByRoomAsync(int roomId) =>
            _db.Seats.Where(s => s.RoomId == roomId).ToListAsync();

        public Task<MovieDto?> GetMovieByIdAsync(int movieId) =>
            _movieService.GetMovieById(movieId);

        public Task<List<Showtime>> GetOverlappingShowtimesAsync(int roomId, DateOnly date) =>
            _db.Showtimes.Where(s => s.RoomId == roomId && s.ShowDate == date).ToListAsync();

        public Task<List<Showtime>> GetAllShowtimesAsync() =>
            _db.Showtimes.Include(s => s.Room).ToListAsync();

        public Task<Showtime?> GetShowtimeByIdAsync(int id) =>
            _db.Showtimes.Include(s => s.Room).FirstOrDefaultAsync(s => s.Id == id);

        public Task<int> CountTotalSeatsAsync(int showtimeId) =>
            _db.SeatShowtimes.CountAsync(s => s.ShowtimeId == showtimeId);

        public Task<int> CountOccupiedSeatsAsync(int showtimeId) =>
            _db.SeatShowtimes.CountAsync(s => s.ShowtimeId == showtimeId && s.Status != SeatStatus.Available);

        public Task<List<OrderDetail>> GetOrderDetailsByShowtimeIdAsync(int showtimeId) =>
            _db.OrderDetails.Where(od => od.ShowtimeId == showtimeId).ToListAsync();

        public Task<List<int>> GetOrderIdsByShowtimeIdAsync(int showtimeId) =>
            _db.OrderDetails.Where(od => od.ShowtimeId == showtimeId).Select(od => od.OrderId).Distinct().ToListAsync();

        public void AddShowtime(Showtime showtime) =>
            _db.Showtimes.Add(showtime);

        public void AddSeatShowtime(SeatShowtime seatShowtime) =>
            _db.SeatShowtimes.Add(seatShowtime);

        public void RemoveSeatShowtimes(int showtimeId)
        {
            var seats = _db.SeatShowtimes.Where(s => s.ShowtimeId == showtimeId);
            _db.SeatShowtimes.RemoveRange(seats);
        }

        public void RemoveShowtime(Showtime showtime) =>
            _db.Showtimes.Remove(showtime);

        public Task SaveChangesAsync() =>
            _db.SaveChangesAsync();
    }

}
