using BookingManagement.Controllers;
using BookingManagement.Models.Entities;
using BookingManagement.Repositories;
using BookingManagement.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Moq;
using VNPAY.NET.Models;

namespace BookingManagementTests
{
    public class VnpayControllerTest
    {
        private readonly Mock<IOrderRepository> _orderRepoMock = new();
        private readonly Mock<IVnpayPaymentService> _vnpayServiceMock = new();
        private readonly Mock<ITransactionService> _transactionServiceMock = new();

        private VnpayController CreateController()
        {
            var controller = new VnpayController(
                _orderRepoMock.Object,
                _vnpayServiceMock.Object,
                _transactionServiceMock.Object
            );

            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.1");
            controller.ControllerContext.HttpContext = httpContext;

            return controller;
        }

        [Fact]
        // Trả về NotFound khi không tìm thấy đơn hàng với ID đã cung cấp.
        public async Task CreateUrl_OrderNotFound_ReturnsNotFound()
        {
            _orderRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Order?)null);

            var controller = CreateController();

            var result = await controller.CreateUrl(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Không tìm thấy đơn hàng", notFound.Value);
        }

        [Fact]
        // Tạo URL thanh toán thành công khi đơn hàng hợp lệ.
        public async Task CreateUrl_ValidOrder_ReturnsPaymentUrl()
        {
            var order = new Order
            {
                Id = 1,
                UserId = 123,
                BookingDate = DateOnly.FromDateTime(DateTime.Today),
                TotalPrice = 150000,
                Status = true
            };

            _orderRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(order);
            _vnpayServiceMock.Setup(service => service.CreatePaymentUrl(150000, "1", "192.168.1.1"))
                             .Returns("https://vnpay.vn/payment?orderId=1");

            var controller = CreateController();

            var result = await controller.CreateUrl(1);

            var ok = Assert.IsType<OkObjectResult>(result);

            var json = JsonSerializer.Serialize(ok.Value);
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            Assert.NotNull(dict);
            Assert.Equal("https://vnpay.vn/payment?orderId=1", dict["paymentUrl"]);
        }

        [Fact]
        // Xử lý callback từ VNPAY thành công → trả về trạng thái success.
        public async Task Callback_SuccessfulPayment_ReturnsSuccess()
        {
            var callbackResult = new PaymentResult
            {
                IsSuccess = true,
                PaymentId = 123,
                Description = "OK"
            };

            _vnpayServiceMock.Setup(s => s.ProcessCallback(It.IsAny<IQueryCollection>()))
                             .Returns(callbackResult);

            _transactionServiceMock.Setup(s => s.SaveTransactionAsync(callbackResult))
                                   .ReturnsAsync(true);

            var controller = CreateController();

            var result = await controller.Callback();

            var ok = Assert.IsType<OkObjectResult>(result);

            var json = JsonSerializer.Serialize(ok.Value);
            var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            Assert.Equal("success", data["status"].GetString());
            Assert.Equal(123, data["orderId"].GetInt32());
        }

        [Fact]
        // Xử lý callback từ VNPAY thất bại → trả về lỗi và thông báo.
        public async Task Callback_FailedPayment_ReturnsBadRequest()
        {
            var callbackResult = new PaymentResult
            {
                IsSuccess = false,
            };

            _vnpayServiceMock.Setup(s => s.ProcessCallback(It.IsAny<IQueryCollection>()))
                             .Returns(callbackResult);

            var controller = CreateController();

            var result = await controller.Callback();

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            var json = JsonSerializer.Serialize(badRequest.Value);
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            Assert.NotNull(dict);
            Assert.Equal("fail", dict["status"]);
            Assert.Equal("Thanh toán thất bại", dict["message"]);
        }

        [Fact]
        // Xử lý IPN hợp lệ (VNPAY gửi kết quả thanh toán về server) → lưu thành công.
        public async Task Ipn_ValidRequest_ReturnsOk()
        {
            var result = new PaymentResult { IsSuccess = true };

            _vnpayServiceMock.Setup(s => s.ProcessCallback(It.IsAny<IQueryCollection>()))
                             .Returns(result);

            _transactionServiceMock.Setup(s => s.SaveTransactionAsync(result))
                                   .ReturnsAsync(true);

            var controller = CreateController();

            var response = await controller.Ipn();

            var ok = Assert.IsType<OkObjectResult>(response);
            Assert.Equal("IPN OK", ok.Value);
        }

        [Fact]
        // Xử lý IPN nhưng lưu transaction thất bại → trả về lỗi.
        public async Task Ipn_SaveTransactionFailed_ReturnsBadRequest()
        {
            var result = new PaymentResult { IsSuccess = true };

            _vnpayServiceMock.Setup(s => s.ProcessCallback(It.IsAny<IQueryCollection>()))
                             .Returns(result);

            _transactionServiceMock.Setup(s => s.SaveTransactionAsync(result))
                                   .ReturnsAsync(false);

            var controller = CreateController();

            var response = await controller.Ipn();

            var badRequest = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal("IPN Failed", badRequest.Value);
        }

        /* -----Test luồng hoạt động------ */
        [Fact]
        // Mô phỏng luồng thanh toán thành công hoàn chỉnh: tạo URL → callback → IPN.
        public async Task CompletePaymentFlow_Success()
        {
            // Step 1: Giả lập đơn đặt hàng hợp lệ
            var order = new Order
            {
                Id = 1,
                UserId = 123,
                BookingDate = DateOnly.FromDateTime(DateTime.Today),
                TotalPrice = 150000,
                Status = false // Chưa thanh toán
            };

            _orderRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(order);

            // Step 2: Tạo URL thanh toán
            _vnpayServiceMock.Setup(service => service.CreatePaymentUrl(150000, "1", "192.168.1.1"))
                             .Returns("https://vnpay.vn/payment?orderId=1");

            var controller = CreateController();
            var createUrlResult = await controller.CreateUrl(1);
            var okUrl = Assert.IsType<OkObjectResult>(createUrlResult);

            var urlJson = JsonSerializer.Serialize(okUrl.Value);
            var urlDict = JsonSerializer.Deserialize<Dictionary<string, string>>(urlJson);
            Assert.NotNull(urlDict);
            Assert.Equal("https://vnpay.vn/payment?orderId=1", urlDict["paymentUrl"]);

            // Step 3: Gọi callback từ VNPAY sau khi thanh toán thành công
            var paymentResult = new PaymentResult
            {
                IsSuccess = true,
                PaymentId = 1,
                Description = "OK"
            };

            _vnpayServiceMock.Setup(s => s.ProcessCallback(It.IsAny<IQueryCollection>()))
                             .Returns(paymentResult);

            _transactionServiceMock.Setup(s => s.SaveTransactionAsync(paymentResult))
                                   .ReturnsAsync(true);

            var callbackResult = await controller.Callback();
            var okCallback = Assert.IsType<OkObjectResult>(callbackResult);
            var callbackJson = JsonSerializer.Serialize(okCallback.Value);
            var callbackData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(callbackJson);
            Assert.Equal("success", callbackData["status"].GetString());
            Assert.Equal(1, callbackData["orderId"].GetInt32());

            // Step 4: Gọi IPN để cập nhật đơn hàng (optional ở một số flow)
            var ipnResult = await controller.Ipn();
            var ipnOk = Assert.IsType<OkObjectResult>(ipnResult);
            Assert.Equal("IPN OK", ipnOk.Value);
        }

        [Fact]
        // Mô phỏng luồng callback thanh toán thất bại.
        public async Task CompletePaymentFlow_FailedPayment_ReturnsError()
        {
            var paymentResult = new PaymentResult
            {
                IsSuccess = false,
                PaymentId = 2,
                Description = "FAILED"
            };

            _vnpayServiceMock.Setup(s => s.ProcessCallback(It.IsAny<IQueryCollection>()))
                             .Returns(paymentResult);

            var controller = CreateController();

            var callbackResult = await controller.Callback();

            var badRequest = Assert.IsType<BadRequestObjectResult>(callbackResult);

            var json = JsonSerializer.Serialize(badRequest.Value);
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            Assert.NotNull(dict);
            Assert.Equal("fail", dict["status"]);
            Assert.Equal("Thanh toán thất bại", dict["message"]);
        }
    }
}