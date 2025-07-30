using BookingManagement.Models.DTOs;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;

namespace BookingManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimeController : ControllerBase
    {
        private readonly IShowtimeService _service;

        public ShowtimeController(IShowtimeService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShowtime([FromBody] CreateShowtimeDto dto) =>
            await _service.CreateShowtime(dto);

        [HttpGet]
        public async Task<IActionResult> GetAllShowtimes() =>
            await _service.GetAllShowtimes();

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShowtimeById(int id) =>
            await _service.GetShowtimeById(id);

        [HttpPut("{id}")]
        public async Task<IActionResult> EditShowtime(int id, [FromBody] EditShowtimeDto dto) =>
            await _service.EditShowtime(id, dto);

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelShowtime(int id) =>
            await _service.CancelShowtime(id);
    }
}
