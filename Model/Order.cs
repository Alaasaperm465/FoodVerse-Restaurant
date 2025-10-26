using Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public enum OrderType
    {
        DineIn,
        Takeout,
        Delivery
    }

    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Preparing,
        Ready,
        Completed,
        Cancelled
    }

    public class Order:BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? EstimatedDeliveryTime { get; set; } 
        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } 

        public string? Notes { get; set; }

        public virtual List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; } = 0;

        [StringLength(100)]
        public string? DiscountDescription { get; set; }
        public OrderType OrderType { get; set; }  
        public string? DeliveryAddress { get; set; }
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }
    }
}
