using MovieBookingWeb.Models;

namespace MovieBookingWeb.Services
{
    public class RoomLayoutService
    {
        private readonly Dictionary<string, Room> roomLayouts;

        public RoomLayoutService()
        {
            roomLayouts = new Dictionary<string, Room>();
            for (int i = 1; i <= 5; i++)
            {
                var roomName = $"Rạp {i}";
                var room = new Room
                {
                    RoomName = roomName,
                    Rows = 9,
                    Columns = 9,
                    Seats = new List<Seat>()
                };

                for (int row = 0; row < room.Rows; row++)
                {
                    for (int col = 0; col < room.Columns; col++)
                    {
                        room.Seats.Add(new Seat
                        {
                            Row = row,
                            Column = col,
                            SeatId = $"{(char)('A' + row)}{col + 1}",
                            IsVIP = (row == 0),
                            IsBroken = false
                        });
                    }
                }
                roomLayouts.Add(roomName, room);
            }
        }

        public Dictionary<string, Room> GetAllRoomLayouts() => roomLayouts;

        public Room? GetRoomLayout(string roomName)
        {
            roomLayouts.TryGetValue(roomName, out var room);
            return room;
        }
    }
}
