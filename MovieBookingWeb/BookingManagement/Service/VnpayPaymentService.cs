using BookingManagement.Models.Entities;
using BookingManagement.Models.VnPayModels;
using Microsoft.Extensions.Options;
using VNPAY.NET;
using VNPAY.NET.Models;

namespace BookingManagement.Service.VnPay
{
    public class VnpayPaymentService : IVnpayPaymentService
    {
        private readonly IVnpay _vnpay;
        private readonly VnpayOptions _options;

        public VnpayPaymentService(IOptions<VnpayOptions> options, IVnpay vnpay)
        {
            _options = options.Value;
            _vnpay = vnpay;

            // Khởi tạo cấu hình VNPAY
            _vnpay.Initialize(
                _options.TmnCode,
                _options.HashSecret,
                _options.BaseUrl,
                _options.CallbackUrl,
                version: "2.1.0",
                orderType: "other"
                );
        }

        public string CreatePaymentUrl(double amount, string orderId, string ipAddress)
        {
            // Tạo PaymentId ngẫu nhiên bằng timestamp (sẽ không trùng)
            long paymentId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var request = new PaymentRequest
            {
                PaymentId = paymentId,
                Money = amount,
                IpAddress = ipAddress,
                Description = $"OrderId:{orderId}" // Lưu orderId thật tại đây để callback đọc lại
            };

            return _vnpay.GetPaymentUrl(request);
        }

        public PaymentResult ProcessCallback(IQueryCollection query)
        {
            try
            {
                return _vnpay.GetPaymentResult(query);
            }
            catch (ArgumentException ex)
            {
                // Ghi log hoặc trả về một PaymentResult thất bại
                return new PaymentResult
                {
                    IsSuccess = false,
                    Description = "Callback không hợp lệ: " + ex.Message
                };
            }
        }

    }
}
