using System;

namespace MovieManagementWeb_API.Models.DTOs
{
    public class ShowtimeDto
    {
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string RoomName { get; set; }
    }
} 