using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Models
{
    public class Category: BaseEntity
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = " Description is required")]
        [StringLength(300, ErrorMessage = "Description cannot be more than 200 characters")]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(1, 1000, ErrorMessage = "Display order must be between 1 and 1000")]
        public int DisplayOrder { get; set; }

        public List<MenuItem>? MenuItems { get; set; }
    }
}
