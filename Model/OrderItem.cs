using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManagement.Models
{
    public class OrderItem:BaseEntity
    {
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }


        [Required(ErrorMessage = "Unit Price is required.")]
        [Range(0.01, 100000, ErrorMessage = "Unit Price must be greater than 0.")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }


        [StringLength(200, ErrorMessage = "Special request cannot more than 200 characters.")]
        public string? SpecialRequest { get; set; }


        [Required(ErrorMessage = "Order ID is required.")]
        public int OrderId { get; set; }

        public Order? Order { get; set; }

        [Required(ErrorMessage = "Menue Item is required.")]
        public int MenuItemId { get; set; }

        public MenuItem MenuItem { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;
    }
}

