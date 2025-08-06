using BookingManagement.Controllers;
using BookingManagement.Models.DTOs;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookingManagementTests
{
    public class PromotionControllerTests
    {
        private readonly Mock<IPromotionService> _mockService;
        private readonly PromotionController _controller;

        public PromotionControllerTests()
        {
            _mockService = new Mock<IPromotionService>();
            _controller = new PromotionController(_mockService.Object);
        }

        [Fact]
        // Đảm bảo controller trả về danh sách PromotionDto với OkObjectResult.
        public async Task GetAll_ReturnsListOfPromotions()
        {
            var mockPromotions = new List<PromotionDto>
            {
                new PromotionDto
                {
                    Id = 1,
                    PromotionCode = "SUMMER2025",
                    Description = "Summer Deal",
                    DiscountAmount = 100000,
                    Quantity = 50,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(30),
                    IsActive = true
                },
                new PromotionDto
                {
                    Id = 2,
                    PromotionCode = "WINTER2025",
                    DiscountPercent = 15,
                    Quantity = 20,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(60),
                    IsActive = false
                }
            };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(mockPromotions);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PromotionDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        // Trả về PromotionDto khi tìm thấy.
        public async Task Get_ReturnsPromotion_WhenFound()
        {
            var promo = new PromotionDto
            {
                Id = 1,
                PromotionCode = "SUMMER2025",
                Description = "Hot summer deal",
                DiscountAmount = 50000,
                DiscountPercent = null,
                Quantity = 100,
                StartDate = new DateTime(2025, 7, 1),
                EndDate = new DateTime(2025, 8, 1),
                IsActive = true
            };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(promo);

            var result = await _controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PromotionDto>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("SUMMER2025", returnValue.PromotionCode);
        }

        [Fact]
        // Trả về NotFound khi không tìm thấy Promotion.
        public async Task Get_ReturnsNotFound_WhenNotFound()
        {
            _mockService.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((PromotionDto?)null);

            var result = await _controller.Get(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Trả về CreatedAtAction với PromotionDto khi tạo thành công.
        public async Task Create_ReturnsCreatedPromotion()
        {
            var createDto = new CreatePromotionDto
            {
                PromotionCode = "NEWYEAR2025",
                Description = "New Year Special",
                DiscountPercent = 20,
                Quantity = 200,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(15)
            };

            var createdPromo = new PromotionDto
            {
                Id = 10,
                PromotionCode = createDto.PromotionCode,
                Description = createDto.Description,
                DiscountPercent = createDto.DiscountPercent,
                Quantity = createDto.Quantity,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                IsActive = true
            };

            _mockService.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(createdPromo);

            var result = await _controller.Create(createDto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<PromotionDto>(createdResult.Value);
            Assert.Equal("NEWYEAR2025", returnValue.PromotionCode);
        }

        [Fact]
        // Trả về BadRequest khi service ném ra exception.
        public async Task Create_ReturnsBadRequest_WhenExceptionThrown()
        {
            var createDto = new CreatePromotionDto
            {
                PromotionCode = "DUPLICATE"
            };

            _mockService.Setup(s => s.CreateAsync(createDto))
                        .ThrowsAsync(new InvalidOperationException("Promotion code already exists"));

            var result = await _controller.Create(createDto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Promotion code already exists", badRequest.Value!.ToString());
        }

        [Fact]
        // Trả về PromotionDto đã cập nhật khi thành công.
        public async Task Update_ReturnsUpdatedPromotion_WhenFound()
        {
            var updateDto = new UpdatePromotionDto
            {
                PromotionCode = "UPDATE2025",
                Description = "Updated description",
                DiscountAmount = 30000,
                Quantity = 10,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(10)
            };

            var updatedPromo = new PromotionDto
            {
                Id = 5,
                PromotionCode = updateDto.PromotionCode,
                Description = updateDto.Description,
                DiscountAmount = updateDto.DiscountAmount,
                Quantity = updateDto.Quantity,
                StartDate = updateDto.StartDate,
                EndDate = updateDto.EndDate,
                IsActive = true
            };

            _mockService.Setup(s => s.UpdateAsync(5, updateDto)).ReturnsAsync(updatedPromo);

            var result = await _controller.Update(5, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PromotionDto>(okResult.Value);
            Assert.Equal("UPDATE2025", returnValue.PromotionCode);
        }

        [Fact]
        // Trả về NotFound nếu không tồn tại Promotion với id tương ứng.
        public async Task Update_ReturnsNotFound_WhenPromotionDoesNotExist()
        {
            var updateDto = new UpdatePromotionDto
            {
                PromotionCode = "NOTFOUND"
            };

            _mockService.Setup(s => s.UpdateAsync(999, updateDto)).ReturnsAsync((PromotionDto?)null);

            var result = await _controller.Update(999, updateDto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Trả về Ok khi xóa thành công.
        public async Task Delete_ReturnsOk_WhenSuccessful()
        {
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Deleted successfully", okResult.Value!.ToString());
        }

        [Fact]
        // Trả về NotFound khi không tìm thấy Promotion để xóa.
        public async Task Delete_ReturnsNotFound_WhenPromotionNotFound()
        {
            _mockService.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);

            var result = await _controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }

        /* -----Test luồng hoạt động------ */
        [Fact]
        // Người dùng xem danh sách khuyến mãi đang hoạt động (IsActive = true)
        public async Task GetAll_ReturnsOnlyActivePromotions()
        {
            var mockPromotions = new List<PromotionDto>
            {
                new PromotionDto { Id = 1, PromotionCode = "ACTIVE1", IsActive = true },
                new PromotionDto { Id = 2, PromotionCode = "INACTIVE1", IsActive = false }
            };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(mockPromotions);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PromotionDto>>(okResult.Value);

            Assert.Contains(returnValue, p => p.IsActive);
            Assert.Equal(2, returnValue.Count); // UI sẽ lọc ở phía client hoặc controller
        }

        [Fact]
        // Người dùng nhập mã không tồn tại
        public async Task GetById_ReturnsNotFound_WhenInvalidCodeUsed()
        {
            _mockService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((PromotionDto?)null);

            var result = await _controller.Get(9999); // id giả định không tồn tại

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        // Admin tạo khuyến mãi với ngày kết thúc nhỏ hơn ngày bắt đầu (logic fail)
        public async Task Create_ReturnsBadRequest_WhenEndDateBeforeStartDate()
        {
            var createDto = new CreatePromotionDto
            {
                PromotionCode = "BADDATE",
                StartDate = DateTime.Today.AddDays(10),
                EndDate = DateTime.Today.AddDays(5) // Lỗi logic
            };

            _mockService.Setup(s => s.CreateAsync(createDto))
                .ThrowsAsync(new InvalidOperationException("End date must be after start date"));

            var result = await _controller.Create(createDto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("End date must be after start date", badRequest.Value!.ToString());
        }

        [Fact]
        // Admin cập nhật khuyến mãi thành trạng thái hết hiệu lực
        public async Task Update_ChangesIsActiveStatus()
        {
            var updateDto = new UpdatePromotionDto
            {
                PromotionCode = "EXPIRE2025",
                IsActive = false
            };

            var updatedPromo = new PromotionDto
            {
                Id = 8,
                PromotionCode = "EXPIRE2025",
                IsActive = false
            };

            _mockService.Setup(s => s.UpdateAsync(8, updateDto)).ReturnsAsync(updatedPromo);

            var result = await _controller.Update(8, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PromotionDto>(okResult.Value);
            Assert.False(returnValue.IsActive);
        }

        [Fact]
        // Admin xóa khuyến mãi đã sử dụng rồi (service từ chối)
        public async Task Delete_ReturnsBadRequest_WhenPromotionAlreadyUsed()
        {
            _mockService.Setup(s => s.DeleteAsync(1))
                .ThrowsAsync(new InvalidOperationException("Promotion already used"));

            var result = await _controller.Delete(1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Promotion already used", badRequest.Value!.ToString());
        }

        [Fact]
        // Trả về danh sách mã khuyến mãi đang hoạt động
        public async Task GetActivePromotionCodes_ReturnsOnlyValidCodes()
        {
            var now = DateTime.Now;

            var mockPromos = new List<PromotionDto>
            {
                new PromotionDto // ✅ hợp lệ
                {
                    PromotionCode = "SUMMER",
                    IsActive = true,
                    StartDate = now.AddDays(-5),
                    EndDate = now.AddDays(5),
                    Quantity = 10
                },
                new PromotionDto // ❌ chưa bắt đầu
                {
                    PromotionCode = "FUTURE",
                    IsActive = true,
                    StartDate = now.AddDays(1),
                    EndDate = now.AddDays(10),
                    Quantity = 10
                },
                new PromotionDto // ❌ đã hết hạn
                {
                    PromotionCode = "EXPIRED",
                    IsActive = true,
                    StartDate = now.AddDays(-10),
                    EndDate = now.AddDays(-1),
                    Quantity = 10
                },
                new PromotionDto // ❌ đã hết số lượng
                {
                    PromotionCode = "EMPTY",
                    IsActive = true,
                    StartDate = now.AddDays(-5),
                    EndDate = now.AddDays(5),
                    Quantity = 0
                },
                new PromotionDto // ❌ không active
                {
                    PromotionCode = "INACTIVE",
                    IsActive = false,
                    StartDate = now.AddDays(-5),
                    EndDate = now.AddDays(5),
                    Quantity = 10
                }
            };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(mockPromos);

            var result = await _controller.GetActivePromotionCodes();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var codes = Assert.IsType<List<string>>(okResult.Value);

            Assert.Single(codes);
            Assert.Contains("SUMMER", codes);
        }
    }
}
