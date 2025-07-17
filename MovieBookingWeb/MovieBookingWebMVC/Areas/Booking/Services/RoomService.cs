using MovieBookingWebMVC.Areas.Booking.Models.DTOs;
using MovieBookingWebMVC.Areas.Booking.Models.ViewModel;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace MovieBookingWebMVC.Areas.Booking.Services
{
    public class RoomService : IRoomService
    {
        private readonly HttpClient _httpClient;

        public RoomService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("ApiClient_Booking");
        }

        // Lấy danh sách phòng chiếu
        public async Task<List<RoomDto>> GetRoomsAsync()
        {
            var res = await _httpClient.GetAsync("/api/room");
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<RoomDto>>(json)!;
        }

        // Lấy thông tin phòng để hiển thị (RoomDto)
        public async Task<RoomDto?> GetRoomByIdAsync(int id)
        {
            var res = await _httpClient.GetAsync($"/api/room/{id}");
            if (!res.IsSuccessStatusCode) return null;

            var json = await res.Content.ReadAsStringAsync();
            Console.WriteLine("📥 JSON RoomDto: " + json);
            return JsonConvert.DeserializeObject<RoomDto>(json);
        }

        // Lấy thông tin phòng để chỉnh sửa (UpdateRoomDto)
        public async Task<UpdateRoomDto?> GetRoomForEditAsync(int id)
        {
            var res = await _httpClient.GetAsync($"/api/room/{id}");
            if (!res.IsSuccessStatusCode) return null;

            var json = await res.Content.ReadAsStringAsync();
            Console.WriteLine("📥 JSON UpdateRoomDto: " + json);
            return JsonConvert.DeserializeObject<UpdateRoomDto>(json);
        }

        // Tạo mới phòng
        public async Task<bool> CreateRoomAsync(CreateRoomDto dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            Console.WriteLine("📤 JSON gửi từ MVC sang API (CreateRoom): " + json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await _httpClient.PostAsync("/api/room", content);

            var responseBody = await res.Content.ReadAsStringAsync();
            Console.WriteLine("📥 Response body: " + responseBody);

            return res.IsSuccessStatusCode;
        }

        // Cập nhật phòng
        public async Task<bool> UpdateRoomAsync(int id, UpdateRoomDto dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            Console.WriteLine("📤 JSON gửi từ MVC sang API (UpdateRoom): " + json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await _httpClient.PutAsync($"/api/room/{id}", content);

            var responseBody = await res.Content.ReadAsStringAsync();
            Console.WriteLine("📥 Response body: " + responseBody);

            return res.IsSuccessStatusCode;
        }

        // Lấy danh sách ghế theo phòng
        public async Task<List<SeatDto>> GetSeatsByRoomIdAsync(int roomId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<SeatDto>>($"/api/seat/room/{roomId}");
                return response ?? new List<SeatDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi khi lấy danh sách ghế: " + ex.Message);
                return new List<SeatDto>();
            }
        }
        public async Task<UpdateRoomViewModel?> GetRoomViewModelForEditAsync(int id)
        {
            var res = await _httpClient.GetAsync($"/api/room/{id}");
            Console.WriteLine($"📡 [Service] Gửi GET /api/room/{id}, Status: {res.StatusCode}");

            if (!res.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ [Service] API trả về lỗi.");
                return null;
            }

            var json = await res.Content.ReadAsStringAsync();
            Console.WriteLine("📥 [Service] JSON trả về:");
            Console.WriteLine(json);

            var dto = JsonConvert.DeserializeObject<UpdateRoomDto>(json);

            if (dto == null)
            {
                Console.WriteLine("❌ [Service] Deserialize JSON thất bại (dto == null)");
                return null;
            }

            Console.WriteLine("✅ [Service] Deserialize thành công:");
            Console.WriteLine($"RoomName: {dto.RoomName}, Rows: {dto.Rows}, Columns: {dto.Columns}");

            var vm = new UpdateRoomViewModel
            {
                Id = id, // <-- Chỗ này cần kiểm tra lại nếu API không trả về Id
                RoomName = dto.RoomName,
                Status = dto.Status,
                Rows = dto.Rows,
                Columns = dto.Columns
            };

            return vm;
        }
        public async Task<bool> UpdateRoomFromViewModelAsync(UpdateRoomViewModel model)
        {
            var payload = new
            {
                roomName = model.RoomName,
                status = model.Status,
                rows = model.Rows,
                columns = model.Columns
            };

            var json = JsonConvert.SerializeObject(payload);
            Console.WriteLine("📤 JSON gửi từ ViewModel sang API (PUT): " + json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await _httpClient.PutAsync($"/api/room/{model.Id}", content);

            var responseBody = await res.Content.ReadAsStringAsync();
            Console.WriteLine("📥 Response body: " + responseBody);

            return res.IsSuccessStatusCode;
        }
        public async Task<RoomDetailsViewModel?> GetRoomDetailsAsync(int roomId)
        {
            var res = await _httpClient.GetAsync($"/api/room/details/{roomId}");
            Console.WriteLine($"📡 [Service] Gửi GET /api/room/details/{roomId}, Status: {res.StatusCode}");

            if (!res.IsSuccessStatusCode)
            {
                Console.WriteLine("❌ [Service] API trả về lỗi.");
                return null;
            }

            var json = await res.Content.ReadAsStringAsync();
            Console.WriteLine("📥 [Service] JSON trả về:");
            Console.WriteLine(json);

            var dto = JsonConvert.DeserializeObject<RoomDetailsDto>(json);
            if (dto == null)
            {
                Console.WriteLine("❌ [Service] Deserialize thất bại (dto == null)");
                return null;
            }

            Console.WriteLine("✅ [Service] Deserialize thành công:");
            Console.WriteLine($"RoomName: {dto.RoomName}, Seats.Count: {dto.Seats?.Count}");

            return new RoomDetailsViewModel
            {
                Id = dto.Id,
                RoomName = dto.RoomName,
                RoomQuantity = dto.RoomQuantity,
                Status = dto.Status,
                Seats = dto.Seats
            };
        }

    }
}
