using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Filter;

namespace RestaurantManagement.Controllers
{
    [Authorize]          
    public class FilterController : Controller
    {
        [HandleError]
        public IActionResult Index()
        {
            throw new Exception("This is a test exception.");
        }
        [AllowAnonymous]   //كل الكونترولرز داخل الفلتر هيكون عليها اوثورايز الا هذا الاكشن 
        public IActionResult About()
        {
            return View();
        }
    }
}
