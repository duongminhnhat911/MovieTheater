using BookingManagement.Controllers;
using BookingManagement.Models.DTOs;
using BookingManagement.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookingManagement.Tests
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
            // Arrange
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

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PromotionDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        // Trả về PromotionDto khi tìm thấy.
        public async Task Get_ReturnsPromotion_WhenFound()
        {
            // Arrange
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

            // Act
            var result = await _controller.Get(1);

            // Assert
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
            // Arrange
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

            // Act
            var result = await _controller.Create(createDto);

            // Assert
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
    }
}
