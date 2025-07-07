using BookingManagement.Models.DTOs;
using BookingManagement.Models.Entities;
using BookingManagement.Models.Entities.Enums;
using BookingManagement.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Controllers
{
    // Controllers/RoomController.cs
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _service;

        public RoomController(IRoomService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(CreateRoomDto dto)
        {
            var result = await _service.CreateRoomAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, UpdateRoomDto dto)
        {
            var room = await _service.UpdateRoomAsync(id, dto);
            if (room == null) return NotFound("Không tìm thấy phòng.");
            return Ok(new
            {
                Message = "Cập nhật phòng thành công.",
                room.Id,
                room.RoomName,
                room.Status
            });
        }


        [HttpGet("statistics/{id}")]
        public async Task<IActionResult> GetRoomUtilization(int id)
        {
            var stats = await _service.GetRoomUtilizationAsync(id);
            return Ok(stats);
        }

        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _service.GetRoomsAsync();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var room = await _service.GetRoomByIdAsync(id);
            if (room == null) return NotFound("Không tìm thấy phòng.");

            var seats = await _service.GetSeatsByRoomIdAsync(room.Id);

            var rows = seats.Select(s => s.SeatRow).Distinct().Count();

            var columns = seats.Select(s =>
            {
                var colStr = s.SeatColumn.ToString(); // đảm bảo là string hoặc convert từ char
                return int.TryParse(colStr, out var c) ? c : 0;
            }).Distinct().Count(c => c > 0);

            var dto = new UpdateRoomDto
            {
                RoomName = room.RoomName,
                Status = room.Status,
                Rows = rows,
                Columns = columns
            };

            return Ok(dto);
        }
        // GET: api/room/details/3
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetRoomDetails(int id)
        {
            var result = await _service.GetRoomDetailsAsync(id);
            if (result == null)
                return NotFound($"Không tìm thấy phòng có ID = {id}");

            return Ok(result);
        }
    }
}