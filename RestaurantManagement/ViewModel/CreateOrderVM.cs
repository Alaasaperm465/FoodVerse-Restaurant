using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModel
{
    public class CreateOrderVM
    {
        public List<OrderItemVM> OrderItems { get; set; } = new List<OrderItemVM>();

        [StringLength(500)]
        public string Notes { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
