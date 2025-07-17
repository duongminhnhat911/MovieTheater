using VNPAY.NET.Models;

namespace BookingManagement.Service
{
    public interface IVnpayPaymentService
    {
        string CreatePaymentUrl(double amount, string orderId, string ipAddress);
        PaymentResult ProcessCallback(IQueryCollection query);
    }
}
