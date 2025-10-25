using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModel
{
    public class OrderItemVM
    {
        [Required]
        public int MenuItemId { get; set; }

        public string MenuItemName { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون على الأقل 1")]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [StringLength(200)]
        public string SpecialInstructions { get; set; }
    }
}
