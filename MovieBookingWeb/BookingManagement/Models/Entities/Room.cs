using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookingManagement.Models.Entities
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        public string? RoomName { get; set; }
        public int RoomQuantity {  get; set; }
        public bool Status { get; set; }

    }
}
