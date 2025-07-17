namespace BookingManagement.Models.VnPayModels
{
    public class VnpayOptions
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
        public string IpnUrl { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }
}
