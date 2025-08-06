using BookingManagement.Controllers;
using BookingManagement.Models.Entities;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text.Json;
using CreateRoomDtoV1 = BookingManagement.Models.DTOs.CreateRoomDto;
using RoomDetailsDtoV1 = BookingManagement.Models.DTOs.RoomDetailsDto;
using UpdateRoomDtoV1 = BookingManagement.Models.DTOs.UpdateRoomDto;

namespace BookingManagementTests
{
    public class RoomControllerTest
    {
        private readonly Mock<IRoomService> _mockService;
        private readonly RoomController _controller;

        public RoomControllerTest()
        {
            _mockService = new Mock<IRoomService>();
            _controller = new RoomController(_mockService.Object);
        }

        [Fact]
        // Kiểm tra tạo phòng mới thành công sẽ trả về OkObjectResult chứa thông tin phòng
        public async Task CreateRoom_ShouldReturnOk_WhenRoomIsCreated()
        {
            var dto = new CreateRoomDtoV1 { RoomName = "Phòng A", Rows = 5, Columns = 6 };
            var createdRoom = new Room { Id = 1, RoomName = "Phòng A", Status = true };

            _mockService.Setup(s => s.CreateRoomAsync(dto)).ReturnsAsync(createdRoom);

            var result = await _controller.CreateRoom(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(createdRoom, ok.Value);
        }

        [Fact]
        // Kiểm tra cập nhật phòng thành công sẽ trả về thông báo và dữ liệu phòng mới
        public async Task UpdateRoom_ShouldReturnOk_WhenRoomExists()
        {
            var dto = new UpdateRoomDtoV1 { RoomName = "Phòng A", Rows = 5, Columns = 6 };
            var updatedRoom = new Room { Id = 2, RoomName = "Phòng B", Status = true };

            _mockService.Setup(s => s.UpdateRoomAsync(2, dto)).ReturnsAsync(updatedRoom);

            var result = await _controller.UpdateRoom(2, dto);

            var ok = Assert.IsType<OkObjectResult>(result);

            // ✅ Parse object thành JSON để kiểm tra thuộc tính
            var json = JsonSerializer.Serialize(ok.Value);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            Assert.Equal("Cập nhật phòng thành công.", root.GetProperty("Message").GetString());
            Assert.Equal(2, root.GetProperty("Id").GetInt32());
            Assert.Equal("Phòng B", root.GetProperty("RoomName").GetString());
            Assert.True(root.GetProperty("Status").GetBoolean());
        }

        [Fact]
        // Kiểm tra khi cập nhật phòng không tồn tại sẽ trả về NotFoundObjectResult
        public async Task UpdateRoom_ShouldReturnNotFound_WhenRoomNotExist()
        {
            var dto = new UpdateRoomDtoV1 { RoomName = "X", Status = false };
            _mockService.Setup(s => s.UpdateRoomAsync(99, dto)).ReturnsAsync((Room)null!);

            var result = await _controller.UpdateRoom(99, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Không tìm thấy phòng.", notFound.Value);
        }

        [Fact]
        // Kiểm tra lấy tỉ lệ sử dụng của phòng sẽ trả về danh sách với thông tin chính xác
        public async Task GetRoomUtilization_ShouldReturnOk_WithExpectedList()
        {
            var expected = new List<object>
            {
                new { RoomId = 3, UtilizationRate = 0.85 }
            };

            _mockService
                .Setup(s => s.GetRoomUtilizationAsync(3))
                .ReturnsAsync(expected.Cast<object>().ToList());

            var result = await _controller.GetRoomUtilization(3);

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<object>>(ok.Value);

            // Nếu bạn muốn check cụ thể giá trị trong phần tử đầu tiên:
            dynamic item = list[0];
            Assert.Equal(3, (int)item.RoomId);
            Assert.Equal(0.85, (double)item.UtilizationRate);
        }

        [Fact]
        // Kiểm tra lấy danh sách tất cả phòng sẽ trả về đúng danh sách
        public async Task GetRooms_ShouldReturnOk_WithList()
        {
            var rooms = new List<Room> {
            new Room { Id = 1, RoomName = "A" },
            new Room { Id = 2, RoomName = "B" }
        };

            _mockService.Setup(s => s.GetRoomsAsync()).ReturnsAsync(rooms);

            var result = await _controller.GetRooms();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(rooms, ok.Value);
        }

        [Fact]
        // Kiểm tra lấy thông tin phòng theo ID thành công sẽ trả về DTO đúng
        public async Task GetRoomById_ShouldReturnOk_WithDto()
        {
            var room = new Room { Id = 1, RoomName = "Phòng A", Status = true };
            var seats = new List<Seat> {
            new Seat { SeatRow = 'A', SeatColumn = '1' },
            new Seat { SeatRow = 'A', SeatColumn = '2' },
            new Seat { SeatRow = 'B', SeatColumn = '1' }
        };

            _mockService.Setup(s => s.GetRoomByIdAsync(1)).ReturnsAsync(room);
            _mockService.Setup(s => s.GetSeatsByRoomIdAsync(1)).ReturnsAsync(seats);

            var result = await _controller.GetRoomById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<UpdateRoomDtoV1>(ok.Value);
            Assert.Equal("Phòng A", dto.RoomName);
            Assert.True(dto.Status);
            Assert.Equal(2, dto.Rows);   // Rows: A, B
            Assert.Equal(2, dto.Columns); // Columns: 1, 2
        }

        [Fact]
        // Kiểm tra lấy phòng theo ID không tồn tại sẽ trả về NotFoundObjectResult
        public async Task GetRoomById_ShouldReturnNotFound_WhenRoomNull()
        {
            _mockService.Setup(s => s.GetRoomByIdAsync(99)).ReturnsAsync((Room)null!);

            var result = await _controller.GetRoomById(99);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Không tìm thấy phòng.", notFound.Value);
        }

        [Fact]
        // Kiểm tra lấy chi tiết phòng sẽ trả về đúng thông tin chi tiết
        public async Task GetRoomDetails_ShouldReturnOk_WhenDataExists()
        {
            var detail = new RoomDetailsDtoV1
            {
                Id = 5,
                RoomName = "Phòng VIP",
                RoomQuantity = 30,
                Status = true,
                Seats = new List<string> { "A1", "A2", "B1" }
            };

            _mockService.Setup(s => s.GetRoomDetailsAsync(5))
                        .ReturnsAsync(detail);
            var result = await _controller.GetRoomDetails(5);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(detail, ok.Value);
        }

        [Fact]
        // Kiểm tra lấy chi tiết phòng với ID không tồn tại sẽ trả về NotFound
        public async Task GetRoomDetails_ShouldReturnNotFound_WhenNull()
        {
            _mockService.Setup(s => s.GetRoomDetailsAsync(999)).ReturnsAsync((RoomDetailsDtoV1)null!);

            var result = await _controller.GetRoomDetails(999);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Không tìm thấy phòng có ID = 999", notFound.Value);
        }

        /* -----Test luồng hoạt động------ */
        [Fact]
        // Tạo phòng mới → Lấy danh sách phòng → Kiểm tra có tồn tại phòng mới
        public async Task CreateRoom_ThenGetRooms_ShouldContainNewRoom()
        {
            var newRoomDto = new CreateRoomDtoV1 { RoomName = "Phòng Test", Rows = 4, Columns = 5 };
            var newRoom = new Room { Id = 10, RoomName = "Phòng Test", Status = true };

            _mockService.Setup(s => s.CreateRoomAsync(newRoomDto)).ReturnsAsync(newRoom);
            _mockService.Setup(s => s.GetRoomsAsync()).ReturnsAsync(new List<Room> { newRoom });

            var createResult = await _controller.CreateRoom(newRoomDto);
            var createOk = Assert.IsType<OkObjectResult>(createResult);
            var created = Assert.IsType<Room>(createOk.Value);
            Assert.Equal("Phòng Test", created.RoomName);

            var listResult = await _controller.GetRooms();
            var listOk = Assert.IsType<OkObjectResult>(listResult);
            var rooms = Assert.IsType<List<Room>>(listOk.Value);
            Assert.Contains(rooms, r => r.RoomName == "Phòng Test");
        }

        [Fact]
        // Cập nhật phòng → Lấy lại thông tin chi tiết → Kiểm tra dữ liệu đã cập nhật
        public async Task UpdateRoom_ThenGetRoomDetails_ShouldReflectChanges()
        {
            var updateDto = new UpdateRoomDtoV1 { RoomName = "Phòng Cập nhật", Rows = 6, Columns = 6, Status = true };
            var updatedRoom = new Room { Id = 11, RoomName = "Phòng Cập nhật", Status = true };
            var roomDetail = new RoomDetailsDtoV1
            {
                Id = 11,
                RoomName = "Phòng Cập nhật",
                RoomQuantity = 36,
                Status = true,
                Seats = new List<string> { "A1", "A2", "A3" }
            };

            _mockService.Setup(s => s.UpdateRoomAsync(11, updateDto)).ReturnsAsync(updatedRoom);
            _mockService.Setup(s => s.GetRoomDetailsAsync(11)).ReturnsAsync(roomDetail);

            var updateResult = await _controller.UpdateRoom(11, updateDto);
            var updateOk = Assert.IsType<OkObjectResult>(updateResult);

            var detailResult = await _controller.GetRoomDetails(11);
            var detailOk = Assert.IsType<OkObjectResult>(detailResult);
            var detail = Assert.IsType<RoomDetailsDtoV1>(detailOk.Value);

            Assert.Equal("Phòng Cập nhật", detail.RoomName);
            Assert.True(detail.Status);
        }

        [Fact]
        // Xem chi tiết phòng → Nếu không tồn tại, báo lỗi
        public async Task ViewRoomDetail_ShouldReturnNotFound_WhenRoomDeleted()
        {
            _mockService.Setup(s => s.GetRoomDetailsAsync(404)).ReturnsAsync((RoomDetailsDtoV1)null!);
            var result = await _controller.GetRoomDetails(404);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Không tìm thấy phòng có ID = 404", notFound.Value);
        }

        [Fact]
        // Xem danh sách → Lấy tỉ lệ sử dụng của phòng đầu tiên
        public async Task ViewRoomList_ThenCheckUtilization_ShouldReturnExpectedRate()
        {
            var roomList = new List<Room>
            {
                new Room { Id = 20, RoomName = "Phòng 20", Status = true }
            };

            var utilization = new List<object>
            {
                new { RoomId = 20, UtilizationRate = 0.7 }
            };

            _mockService.Setup(s => s.GetRoomsAsync()).ReturnsAsync(roomList);
            _mockService.Setup(s => s.GetRoomUtilizationAsync(20))
                        .ReturnsAsync(utilization.Cast<object>().ToList());

            var listResult = await _controller.GetRooms();
            var listOk = Assert.IsType<OkObjectResult>(listResult);
            var rooms = Assert.IsType<List<Room>>(listOk.Value);
            var roomId = rooms.First().Id;

            var utilResult = await _controller.GetRoomUtilization(roomId);
            var utilOk = Assert.IsType<OkObjectResult>(utilResult);
            var utilList = Assert.IsType<List<object>>(utilOk.Value);

            dynamic item = utilList[0];
            Assert.Equal(20, (int)item.RoomId);
            Assert.Equal(0.7, (double)item.UtilizationRate);
        }
    }
}