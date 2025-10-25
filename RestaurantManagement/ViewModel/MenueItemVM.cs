using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.ViewModel
{
    public class MenuItemVM
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(4, ErrorMessage = "Name cannot be less than 4 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(50, ErrorMessage = "Description cannot be more than 50 characters")]
        [MinLength(15, ErrorMessage = "Description cannot be less than 15 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(10, double.MaxValue, ErrorMessage = "Price must be greater than 10")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        // دا اللي هيتخزن في الداتابيز بعد الرفع
        public string? ImageUrl { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }

        [Range(1, 120, ErrorMessage = "Preparation Time must be between 1 and 120 minutes")]
        public int PreparationTime { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public SelectList? Categories { get; set; }

        [Required(ErrorMessage = "Instoke is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Instoke cannot be negative")]
        public int Instoke { get; set; }
    }
}
