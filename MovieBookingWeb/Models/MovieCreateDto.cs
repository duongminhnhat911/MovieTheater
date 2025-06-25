using System;
using System.Collections.Generic;

namespace MovieBookingWeb.Models
{
    // These DTOs match the structure expected by the MovieManagementWeb_API
    // to ensure clean data transfer.

    public class ShowtimeCreateDto
    {
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string RoomName { get; set; }
    }

    public class MovieCreateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string CarouselImage { get; set; }
        public string TrailerLink { get; set; }
        public string Subtitle { get; set; }
        public string RatingCode { get; set; }
        public int? Duration { get; set; }
        public List<string> Genres { get; set; }
        public string Director { get; set; }
        public string Cast { get; set; }
        public string Description { get; set; }
        public string ProductionCompany { get; set; }
        public List<string> Format { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public List<ShowtimeCreateDto> Showtimes { get; set; }
        public string EditedByUsername { get; set; }
        public string? Status { get; set; }
    }
} 
