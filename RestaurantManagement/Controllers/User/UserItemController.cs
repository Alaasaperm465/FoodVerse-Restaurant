using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace RestaurantManagement.Controllers.User
{
    public class UserItemController : Controller
    {
        private readonly IMenuItemService _menuService;
        private readonly ICategoryService _categoryService;
        public UserItemController(IMenuItemService menuItemService,ICategoryService categoryService)
        {
            _menuService = menuItemService; 
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            var menuItems = await _menuService.GetAllAsync();
            var instock = menuItems.Where(i => i.Instoke > 0).ToList();
            return View(instock);
        }
    }
}
