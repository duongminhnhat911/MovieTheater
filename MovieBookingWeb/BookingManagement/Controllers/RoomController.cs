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
            if (room == null) return NotFound();
            return Ok(room);
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
            return Ok(room);
        }
    }
}