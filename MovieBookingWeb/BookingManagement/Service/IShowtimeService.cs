using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.Service
{
    public interface IShowtimeService
    {
        Task<IActionResult> CreateShowtime(CreateShowtimeDto dto);
        Task<IActionResult> GetAllShowtimes();
        Task<IActionResult> GetShowtimeById(int id);
        Task<IActionResult> EditShowtime(int id, EditShowtimeDto dto);
        Task<IActionResult> CancelShowtime(int id);
    }
}
