using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace RestaurantManagement.Controllers
{
    public class UserController : Controller
    {
        private readonly IMenuItemService _itemService;
        private readonly ICategoryService _categoryService;
        public UserController(IMenuItemService itemService, ICategoryService categoryService)
        {
            _itemService = itemService;
            _categoryService = categoryService;
        }
        //iaction category
        //---show cat
        //---Details cat
        //---Order Cat
        public async Task<IActionResult> showCats()
        {
            var cats = await _categoryService.GetAllAsync(); 
            return View(cats);
        }
        //IAction menue item 
        //---show item
        //---Details item
        //---Order item
    }
}
