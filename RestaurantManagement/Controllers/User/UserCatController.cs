using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace RestaurantManagement.Controllers.User
{
    public class UserCatController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMenuItemService _menuItemService;
        public UserCatController(ICategoryService category, IMenuItemService menuItem)
        {
            _categoryService = category;
            _menuItemService = menuItem;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            var categoriesWithItems = categories.Where(c => c.MenuItems != null && c.MenuItems.Any()).ToList();
            return View(categoriesWithItems);
        }
        public async Task<IActionResult> Items(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            var items = await _menuItemService.GetAllAsync();
            var categoryItems = items.Where(i => i.CategoryId == id  && !i.IsDeleted).ToList();

            ViewBag.CategoryName = category.Name;
            return View(categoryItems);
        }
    }
}
