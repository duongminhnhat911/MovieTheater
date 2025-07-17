using System.ComponentModel.DataAnnotations.Schema;

namespace BookingManagement.Models.Entities
{
    public class Transaction
    {
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public int PaymentId { get; set; }
        public DateOnly TransactionDate { get; set; }
        public int Price { get; set; }
        public bool Status { get; set; }
    }
}
